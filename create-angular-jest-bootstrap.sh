#!/bin/bash

# Check if jq is installed
if ! command -v jq &> /dev/null; then
    echo "Error: jq is not installed. Install it using 'sudo apt install jq' (Linux) or 'brew install jq' (macOS)."
    exit 1
fi

# Check if Angular CLI is installed
if ! command -v ng &> /dev/null; then
    echo "Error: Angular CLI is not installed. Install it using 'npm install -g @angular/cli'."
    exit 1
fi

# check that angular is version 19.2 or higher
ANGULAR_VERSION=$(ng --version)

echo "Angular CLI version: $ANGULAR_VERSION"

if [[ "$ANGULAR_VERSION" != 19.2* ]]; then
    echo "Error: Angular CLI version must start with 19.2. You are using version $ANGULAR_VERSION."
    exit 1
fi

# Disable Angular CLI analytics globally
echo "Disabling Angular CLI analytics globally..."
ng config --global cli.analytics false

# Project Name (first argument, default: my-angular-app)
PROJECT_NAME=${1:-my-angular-app}

echo "Creating Angular project: $PROJECT_NAME"

# Step 1: Create a new Angular project with default settings
ng new "$PROJECT_NAME" --defaults --skip-git
cd "$PROJECT_NAME" || exit

# Step 2: Initialize Git
echo "Initializing Git repository..."
git init
git add .
git commit -m "Initial commit after create"

# Step 3: Disable Angular CLI analytics
# Disable Angular CLI analytics
echo "Disabling Angular CLI analytics..."
ng analytics off

git add .
git stage .

# commit the changes
git commit -m "Disabled Angular CLI analytics"

# Step 4: Remove Karma and Jasmine dependencies
echo "Removing Karma and Jasmine..."
# npm uninstall karma karma-chrome-launcher karma-coverage karma-jasmine karma-jasmine-html-reporter jasmine-core jasmine-spec-reporter
npm remove karma karma-chrome-launcher karma-coverage-istanbul-reporter karma-jasmine karma-jasmine-html-reporter

# Step 5: Add Jest and configure it
echo "Installing Jest..."
npm install --save-dev jest jest-preset-angular @types/jest ts-jest @angular-builders/jest ts-node

# Step 6: Remove Karma configuration files
echo "Cleaning up Karma files..."
rm -f karma.conf.js src/test.ts

# Step 7: Update package.json scripts
echo "Updating package.json..."
jq '.scripts.test="jest --config jest.config.ts"' package.json > package.tmp.json && mv package.tmp.json package.json

jq '.scripts["test:watch"] = "jest --watch --config jest.config.ts"' package.json > package.tmp.json && mv package.tmp.json package.json

# Step 8: Update angular.json to use Jest
echo "Updating angular.json..."

jq '.projects |= with_entries(
    .value.architect.test.builder = "@angular-builders/jest:run" |
    .value.architect.test.options = {} |
    .value.architect.test.options.configPath = "jest.config.ts"
  )
' angular.json > angular.tmp.json && mv angular.tmp.json angular.json

echo "Generating jest.config.ts for Angular..."

cat <<EOL > jest.config.ts
export default {
  preset: 'jest-preset-angular',
  setupFilesAfterEnv: ['<rootDir>/setup-jest.ts'],
  testMatch: ['**/+(*.)+(spec).+(ts)?(x)'],
  transform: {
    '^.+\\.(ts|mjs|js|html)$': [
      'ts-jest',
      {
        tsconfig: 'tsconfig.spec.json',
        useESM: true, // Ensures Jest handles ES modules
      },
    ],
  },
  moduleNameMapper: {
    '@angular/core/testing': '<rootDir>/node_modules/@angular/core/fesm2022/testing.mjs',
  },
  extensionsToTreatAsEsm: ['.ts', '.mts'],
  transformIgnorePatterns: ['node_modules/(?!.*\\.mjs$)'],
};
EOL

echo "jest.config.ts has been created"



echo "Generating empty setup-jest.ts..."

cat <<EOL > setup-jest.ts
EOL

echo "Empty setup-jest.ts has been created"

git add .
git stage .

git commit -m "Modified angular.json to use Jest"


# Step 9: Add bootstrap and popper
echo "Adding bootstrap and popper..."
npm install bootstrap @popperjs/core

# Step 10: Update angular.json to include bootstrap and popper

echo "Updating angular.json to include bootstrap and popper..."

jq '.projects |= with_entries(
    .value.architect.build.options.styles = [
      "src/styles.css",
      "node_modules/bootstrap/dist/css/bootstrap.min.css"
    ] |
    .value.architect.build.options.scripts = [
      "node_modules/@popperjs/core/dist/umd/popper.min.js",
      "node_modules/bootstrap/dist/js/bootstrap.bundle.min.js"
    ]
  )
' angular.json > angular.tmp.json && mv angular.tmp.json angular.json

git add .
git stage .

git commit -m "Added bootstrap and popper"

echo "✅ Angular project '$PROJECT_NAME' is set up with Jest and bootstrap."