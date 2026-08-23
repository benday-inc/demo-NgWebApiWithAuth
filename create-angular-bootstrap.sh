#!/bin/bash
#
# Creates a new Angular 22 app using the *default* unit testing setup (Vitest)
# and adds Bootstrap.
#
# As of Angular 21, the Angular team standardized on Vitest as the default test
# runner for new projects (builder: @angular/build:unit-test). There is no
# longer any reason to swap in Jest, so -- unlike the old
# create-angular-jest-bootstrap.sh -- this script does NOT touch the test
# configuration. `ng test` just works.
#
# Usage: ./create-angular-bootstrap.sh [project-name]

set -e

REQUIRED_NG_MAJOR=22
REQUIRED_NODE_MAJOR=22

# ---------------------------------------------------------------------------
# Prerequisite checks
# ---------------------------------------------------------------------------

for tool in node npm git; do
    if ! command -v "$tool" &> /dev/null; then
        echo "Error: $tool is not installed."
        exit 1
    fi
done

NODE_MAJOR=$(node -p "process.versions.node.split('.')[0]")
if [ "$NODE_MAJOR" -lt "$REQUIRED_NODE_MAJOR" ]; then
    echo "Error: Angular $REQUIRED_NG_MAJOR requires Node.js $REQUIRED_NODE_MAJOR or later. You are running $(node -v)."
    exit 1
fi

if ! command -v ng &> /dev/null; then
    echo "Error: Angular CLI is not installed. Install it using 'npm install -g @angular/cli@$REQUIRED_NG_MAJOR'."
    exit 1
fi

NG_VERSION=$(ng version 2>/dev/null | sed -n 's/.*Angular CLI *: *\([0-9][0-9.]*\).*/\1/p' | head -1)
NG_MAJOR=${NG_VERSION%%.*}

echo "Angular CLI version: $NG_VERSION"

if [ "$NG_MAJOR" != "$REQUIRED_NG_MAJOR" ]; then
    echo "Error: Angular CLI version must be $REQUIRED_NG_MAJOR.x. You are using version $NG_VERSION."
    echo "       Run 'npm install -g @angular/cli@$REQUIRED_NG_MAJOR' to upgrade."
    exit 1
fi

# ---------------------------------------------------------------------------
# Step 1: Turn off Angular CLI analytics globally
# ---------------------------------------------------------------------------

echo "Disabling Angular CLI analytics globally..."
ng analytics disable --global

# ---------------------------------------------------------------------------
# Step 2: Create the app
# ---------------------------------------------------------------------------

PROJECT_NAME=${1:-my-angular-app}

echo "Creating Angular project: $PROJECT_NAME"

# Flags are passed explicitly even where they match the v22 defaults, so the
# script keeps producing the same app if the defaults move again.
#   --style=css        plain CSS stylesheets (use scss/tailwind if you prefer)
#   --routing          set up the router
#   --ssr=false        client-side app only
#   --zoneless         no zone.js (the v22 default)
#   --ai-config=none   don't generate CLAUDE.md / AGENTS.md / MCP config
#   --skip-git         we init the repo ourselves below, one commit per step
# NOTE: there is deliberately no test-runner flag here. Omitting it gives you
# the default, which is Vitest. Add --test-runner=karma for the old behavior.
ng new "$PROJECT_NAME" \
    --defaults \
    --skip-git \
    --style=css \
    --routing \
    --ssr=false \
    --zoneless \
    --package-manager=npm \
    --ai-config=none

cd "$PROJECT_NAME" || exit 1

# ---------------------------------------------------------------------------
# Step 3: Initialize Git
# ---------------------------------------------------------------------------

echo "Initializing Git repository..."
git init
git add .
git commit -m "Initial commit after create"

# ---------------------------------------------------------------------------
# Step 4: Turn off analytics for this workspace too
# ---------------------------------------------------------------------------

echo "Disabling Angular CLI analytics for this workspace..."
ng analytics disable

git add .
git commit -m "Disabled Angular CLI analytics"

# ---------------------------------------------------------------------------
# Step 5: Add test scripts + code coverage support
# ---------------------------------------------------------------------------

# `ng test` watches by default in a TTY, so CI needs --no-watch. Coverage with
# the Vitest runner requires @vitest/coverage-v8 to be installed explicitly.
echo "Installing @vitest/coverage-v8..."
VITEST_VERSION=$(node -p "require('./package.json').devDependencies.vitest")
npm install --save-dev "@vitest/coverage-v8@$VITEST_VERSION"

echo "Adding test scripts to package.json..."

cat > .patch-package-json.mjs <<'PATCH_EOF'
import { readFileSync, writeFileSync } from 'node:fs';

const pkg = JSON.parse(readFileSync('package.json', 'utf8'));

pkg.scripts = {
  ...pkg.scripts,
  'test': 'ng test',
  'test:watch': 'ng test --watch',
  'test:ci': 'ng test --no-watch',
  'test:coverage': 'ng test --no-watch --coverage',
};

writeFileSync('package.json', JSON.stringify(pkg, null, 2) + '\n');
PATCH_EOF

node .patch-package-json.mjs
rm .patch-package-json.mjs

git add .
git commit -m "Added test and coverage scripts"

# ---------------------------------------------------------------------------
# Step 6: Add Bootstrap
# ---------------------------------------------------------------------------

# bootstrap.bundle.min.js already includes Popper, so @popperjs/core does not
# need to be installed or referenced separately.
echo "Adding bootstrap..."
npm install bootstrap

echo "Updating angular.json to include bootstrap..."

cat > .patch-angular-json.mjs <<'PATCH_EOF'
import { readFileSync, writeFileSync } from 'node:fs';

const angularJson = JSON.parse(readFileSync('angular.json', 'utf8'));

const BOOTSTRAP_CSS = 'node_modules/bootstrap/dist/css/bootstrap.min.css';
const BOOTSTRAP_JS = 'node_modules/bootstrap/dist/js/bootstrap.bundle.min.js';

for (const project of Object.values(angularJson.projects ?? {})) {
  const options = project.architect?.build?.options;
  if (!options) {
    continue;
  }

  // Bootstrap goes first so that the app's own stylesheet can override it.
  options.styles = [BOOTSTRAP_CSS, ...(options.styles ?? []).filter((s) => s !== BOOTSTRAP_CSS)];
  options.scripts = [...(options.scripts ?? []).filter((s) => s !== BOOTSTRAP_JS), BOOTSTRAP_JS];

  // Bootstrap adds ~300 kB to the initial bundle, which trips the default
  // 500 kB budget warning on a production build. Leave room for it.
  for (const budget of project.architect.build.configurations?.production?.budgets ?? []) {
    if (budget.type === 'initial') {
      budget.maximumWarning = '1MB';
      budget.maximumError = '2MB';
    }
  }
}

writeFileSync('angular.json', JSON.stringify(angularJson, null, 2) + '\n');
PATCH_EOF

node .patch-angular-json.mjs
rm .patch-angular-json.mjs

git add .
git commit -m "Added bootstrap"

# ---------------------------------------------------------------------------
# Done
# ---------------------------------------------------------------------------

echo ""
echo "✅ Angular project '$PROJECT_NAME' is ready."
echo ""
echo "   cd $PROJECT_NAME"
echo "   npm start            # ng serve"
echo "   npm test             # ng test  (Vitest, watches in a terminal)"
echo "   npm run test:ci      # single run, no watch"
echo "   npm run test:coverage"
echo ""
