# PowerShell Script to Create Angular Project with Jest Testing
# Supports Angular CLI 19.2+ and Angular 20
#
# Note: Karma is deprecated in Angular but still included by default.
# This script removes Karma and configures Jest as the test runner.
# Jest provides better performance, better error messages, and is more
# familiar to developers coming from other JavaScript/TypeScript projects.

# Check if jq is installed
if (-not (Get-Command jq -ErrorAction SilentlyContinue)) {
    Write-Host "Error: jq is not installed. Install it using 'winget install jqlang.jq' (Windows) or 'brew install jq' (macOS)." -ForegroundColor Red
    exit 1
}

# Check if Angular CLI is installed
if (-not (Get-Command ng -ErrorAction SilentlyContinue)) {
    Write-Host "Error: Angular CLI is not installed. Install it using 'npm install -g @angular/cli'." -ForegroundColor Red
    exit 1
}

# Check Angular CLI version
$angularVersion = ng version | Select-String -Pattern "Angular CLI: (\d+\.\d+)" | ForEach-Object { $_.Matches.Groups[1].Value }

Write-Host "Angular CLI version: $angularVersion"

# Accept both Angular 19.2+ and 20.x versions
if ($angularVersion -notlike "19.2*" -and $angularVersion -notlike "20.*") {
    Write-Host "Error: Angular CLI version must be 19.2+ or 20.x. You are using version $angularVersion." -ForegroundColor Red
    Write-Host "Note: This script supports Angular 19.2+ and Angular 20." -ForegroundColor Yellow
    exit 1
}

# Disable Angular CLI analytics globally
Write-Host "Disabling Angular CLI analytics globally..."
ng config --global cli.analytics false

# Project Name (first argument, default: my-angular-app)
$projectName = $args[0]
if (-not $projectName) { $projectName = "my-angular-app" }

Write-Host "Creating Angular project: $projectName"

# Step 1: Create a new Angular project with default settings
ng new $projectName --defaults --skip-git
Set-Location $projectName

# Step 2: Initialize Git
Write-Host "Initializing Git repository..."
git init
git add .
git commit -m "Initial commit after create"

# Step 3: Disable Angular CLI analytics
Write-Host "Disabling Angular CLI analytics..."
ng analytics off
git add .
git commit -m "Disabled Angular CLI analytics"

# Step 4: Remove Karma and Jasmine dependencies
Write-Host "Removing Karma and Jasmine..."
npm remove karma karma-chrome-launcher karma-coverage-istanbul-reporter karma-jasmine karma-jasmine-html-reporter

# Step 5: Add Jest and configure it
Write-Host "Installing Jest..."
npm install --save-dev jest jest-preset-angular @types/jest ts-jest @angular-builders/jest ts-node

# Step 6: Remove Karma configuration files
Write-Host "Cleaning up Karma files..."
Remove-Item -Path "karma.conf.js", "src/test.ts" -ErrorAction SilentlyContinue

# Step 7: Update package.json scripts
Write-Host "Updating package.json..."
(Get-Content package.json) -join "`n" | jq '.scripts.test="jest --config jest.config.ts"' | Out-File package.json -Encoding utf8
(Get-Content package.json) -join "`n" | jq '.scripts["test:watch"] = "jest --watch --config jest.config.ts"' | Out-File package.json -Encoding utf8

# Step 8: Update angular.json to use Jest
Write-Host "Updating angular.json..."
(Get-Content angular.json) -join "`n" | jq '.projects |= with_entries(.value.architect.test.builder = "@angular-builders/jest:run" | .value.architect.test.options = {} | .value.architect.test.options.configPath = "jest.config.ts")' | Out-File angular.json -Encoding utf8

# Step 9: Generate jest.config.ts
Write-Host "Generating jest.config.ts for Angular..."

@'
export default {
  preset: 'jest-preset-angular',
  setupFilesAfterEnv: ['<rootDir>/setup-jest.ts'],
  testMatch: ['**/+(*.)+(spec).+(ts)?(x)'],
  transform: {
    '^.+\.(ts|mjs|js|html)$': [
      'ts-jest',
      {
        tsconfig: 'tsconfig.spec.json',
        useESM: true, // Ensures Jest handles ES modules
      },
    ],
  },
  moduleNameMapper: {
    // Updated for Angular 20 - using fesm2022 format
    '@angular/core/testing': '<rootDir>/node_modules/@angular/core/fesm2022/testing.mjs',
    '@angular/(.*)': '<rootDir>/node_modules/@angular/$1',
  },
  extensionsToTreatAsEsm: ['.ts', '.mts'],
  transformIgnorePatterns: ['node_modules/(?!.*\.mjs$)'],
  // Additional settings for Angular 20 compatibility
  moduleFileExtensions: ['ts', 'js', 'html', 'mjs'],
  collectCoverageFrom: [
    'src/**/*.{ts,js}',
    '!src/**/*.spec.{ts,js}',
    '!src/**/main.ts',
    '!src/**/environment*.ts'
  ],
};
'@ | Out-File jest.config.ts -Encoding utf8

Write-Host "jest.config.ts has been created"

# Step 10: Generate setup-jest.ts
Write-Host "Generating empty setup-jest.ts..."
"" | Out-File setup-jest.ts -Encoding utf8

Write-Host "Empty setup-jest.ts has been created"

git add .
git commit -m "Modified angular.json to use Jest"

Write-Host "✅ Angular project '$projectName' is set up with Jest." -ForegroundColor Green
Write-Host "   - Karma has been removed (deprecated in Angular)" -ForegroundColor Cyan
Write-Host "   - Jest is now configured as the test runner" -ForegroundColor Cyan
Write-Host "   - Use 'ng test' to run your tests" -ForegroundColor Yellow
Write-Host "   - Use 'ng test --watch' for continuous testing" -ForegroundColor Yellow