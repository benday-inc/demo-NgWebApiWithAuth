# Set project name (default: my-angular-app)
param (
    [string]$ProjectName = "my-angular-app"
)

Write-Host "Creating Angular project: $ProjectName"

# Step 1: Create a new Angular project
ng new $ProjectName --defaults --skip-git
Set-Location $ProjectName

# Step 2: Remove Karma and Jasmine dependencies
Write-Host "Removing Karma and Jasmine..."
npm uninstall karma karma-chrome-launcher karma-coverage `
    karma-jasmine karma-jasmine-html-reporter jasmine-core jasmine-spec-reporter

# Step 3: Add Jest and configure it
Write-Host "Installing Jest..."
ng add @angular-builders/jest

# Step 4: Remove Karma configuration files
Write-Host "Cleaning up Karma files..."
Remove-Item -Path karma.conf.js -ErrorAction Ignore
Remove-Item -Path src/test.ts -ErrorAction Ignore

# Step 5: Update package.json to use Jest for testing
Write-Host "Updating package.json..."
$packageJson = Get-Content package.json | ConvertFrom-Json
$packageJson.scripts.test = "jest"
$packageJson | ConvertTo-Json -Depth 10 | Set-Content package.json

# Step 6: Initialize Git
Write-Host "Initializing Git repository..."
git init
git add .
git commit -m "Initial commit with Jest setup"

Write-Host "✅ Angular project '$ProjectName' is set up with Jest! Use 'ng test' or 'npm test' to run your tests."
