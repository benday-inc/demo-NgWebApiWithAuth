#
# Creates a new Angular 22 app using the *default* unit testing setup (Vitest)
# and adds Bootstrap.
#
# As of Angular 21, the Angular team standardized on Vitest as the default test
# runner for new projects (builder: @angular/build:unit-test). There is no
# longer any reason to swap in Jest, so -- unlike the old
# create-angular-jest-bootstrap.ps1 -- this script does NOT touch the test
# configuration. `ng test` just works.
#
# Usage: ./create-angular-bootstrap.ps1 [project-name]

$ErrorActionPreference = "Stop"

$RequiredNgMajor = 22
$RequiredNodeMajor = 22

function Assert-ExitCode {
    param([string]$What)
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Error: $What failed with exit code $LASTEXITCODE." -ForegroundColor Red
        exit $LASTEXITCODE
    }
}

function Write-Utf8NoBom {
    param([string]$Path, [string]$Content)
    [System.IO.File]::WriteAllText((Join-Path (Get-Location) $Path), $Content)
}

# ---------------------------------------------------------------------------
# Prerequisite checks
# ---------------------------------------------------------------------------

foreach ($tool in @("node", "npm", "git")) {
    if (-not (Get-Command $tool -ErrorAction SilentlyContinue)) {
        Write-Host "Error: $tool is not installed." -ForegroundColor Red
        exit 1
    }
}

$nodeMajor = [int](node -p "process.versions.node.split('.')[0]")
if ($nodeMajor -lt $RequiredNodeMajor) {
    Write-Host "Error: Angular $RequiredNgMajor requires Node.js $RequiredNodeMajor or later. You are running $(node -v)." -ForegroundColor Red
    exit 1
}

if (-not (Get-Command ng -ErrorAction SilentlyContinue)) {
    Write-Host "Error: Angular CLI is not installed. Install it using 'npm install -g @angular/cli@$RequiredNgMajor'." -ForegroundColor Red
    exit 1
}

$ngVersion = ng version | Select-String -Pattern "Angular CLI\s*:\s*(\d+\.\d+\.\d+)" | ForEach-Object { $_.Matches.Groups[1].Value } | Select-Object -First 1
$ngMajor = $ngVersion.Split(".")[0]

Write-Host "Angular CLI version: $ngVersion"

if ($ngMajor -ne "$RequiredNgMajor") {
    Write-Host "Error: Angular CLI version must be $RequiredNgMajor.x. You are using version $ngVersion." -ForegroundColor Red
    Write-Host "       Run 'npm install -g @angular/cli@$RequiredNgMajor' to upgrade." -ForegroundColor Red
    exit 1
}

# ---------------------------------------------------------------------------
# Step 1: Turn off Angular CLI analytics globally
# ---------------------------------------------------------------------------

Write-Host "Disabling Angular CLI analytics globally..."
ng analytics disable --global

# ---------------------------------------------------------------------------
# Step 2: Create the app
# ---------------------------------------------------------------------------

$projectName = $args[0]
if (-not $projectName) { $projectName = "my-angular-app" }

Write-Host "Creating Angular project: $projectName"

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
ng new $projectName `
    --defaults `
    --skip-git `
    --style=css `
    --routing `
    --ssr=false `
    --zoneless `
    --package-manager=npm `
    --ai-config=none
Assert-ExitCode "ng new"

Set-Location $projectName

# ---------------------------------------------------------------------------
# Step 3: Initialize Git
# ---------------------------------------------------------------------------

Write-Host "Initializing Git repository..."
git init
git add .
git commit -m "Initial commit after create"

# ---------------------------------------------------------------------------
# Step 4: Turn off analytics for this workspace too
# ---------------------------------------------------------------------------

Write-Host "Disabling Angular CLI analytics for this workspace..."
ng analytics disable

git add .
git commit -m "Disabled Angular CLI analytics"

# ---------------------------------------------------------------------------
# Step 5: Add test scripts + code coverage support
# ---------------------------------------------------------------------------

# `ng test` watches by default in a TTY, so CI needs --no-watch. Coverage with
# the Vitest runner requires @vitest/coverage-v8 to be installed explicitly.
Write-Host "Installing @vitest/coverage-v8..."
$vitestVersion = node -p "require('./package.json').devDependencies.vitest"
npm install --save-dev "@vitest/coverage-v8@$vitestVersion"
Assert-ExitCode "npm install @vitest/coverage-v8"

Write-Host "Adding test scripts to package.json..."

$patchPackageJson = @'
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
'@

Write-Utf8NoBom ".patch-package-json.mjs" $patchPackageJson
node .patch-package-json.mjs
Assert-ExitCode "package.json patch"
Remove-Item .patch-package-json.mjs -Force

git add .
git commit -m "Added test and coverage scripts"

# ---------------------------------------------------------------------------
# Step 6: Add Bootstrap
# ---------------------------------------------------------------------------

# bootstrap.bundle.min.js already includes Popper, so @popperjs/core does not
# need to be installed or referenced separately.
Write-Host "Adding bootstrap..."
npm install bootstrap
Assert-ExitCode "npm install bootstrap"

Write-Host "Updating angular.json to include bootstrap..."

$patchAngularJson = @'
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
'@

Write-Utf8NoBom ".patch-angular-json.mjs" $patchAngularJson
node .patch-angular-json.mjs
Assert-ExitCode "angular.json patch"
Remove-Item .patch-angular-json.mjs -Force

git add .
git commit -m "Added bootstrap"

# ---------------------------------------------------------------------------
# Done
# ---------------------------------------------------------------------------

Write-Host ""
Write-Host "✅ Angular project '$projectName' is ready."
Write-Host ""
Write-Host "   cd $projectName"
Write-Host "   npm start            # ng serve"
Write-Host "   npm test             # ng test  (Vitest, watches in a terminal)"
Write-Host "   npm run test:ci      # single run, no watch"
Write-Host "   npm run test:coverage"
Write-Host ""
