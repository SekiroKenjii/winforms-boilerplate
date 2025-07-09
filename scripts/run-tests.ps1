#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Runs all unit tests for the WinForms Boilerplate project.

.DESCRIPTION
    This script executes all unit tests in the solution and provides
    a summary of the test results.

.PARAMETER Configuration
    The build configuration to use (Debug or Release). Default is Debug.

.PARAMETER Verbosity
    The MSBuild verbosity level. Default is minimal.

.EXAMPLE
    .\run-tests.ps1
    Runs all tests in Debug configuration

.EXAMPLE
    .\run-tests.ps1 -Configuration Release -Verbosity normal
    Runs all tests in Release configuration with normal verbosity
#>

param(
    [Parameter()]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",

    [Parameter()]
    [ValidateSet("quiet", "minimal", "normal", "detailed", "diagnostic")]
    [string]$Verbosity = "minimal"
)

Write-Host "🧪 Running Unit Tests for WinForms Boilerplate" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Verbosity: $Verbosity" -ForegroundColor Yellow
Write-Host ""

# Ensure we're in the root directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$rootPath = Split-Path -Parent $scriptPath
Set-Location $rootPath

# Build the solution first
Write-Host "🔨 Building solution..." -ForegroundColor Green
dotnet build --configuration $Configuration --verbosity $Verbosity
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed. Exiting." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "🏃‍♂️ Running tests..." -ForegroundColor Green

# Run all tests
dotnet test --configuration $Configuration --verbosity $Verbosity --no-build

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ All tests passed!" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "❌ Some tests failed. Check the output above for details." -ForegroundColor Red
}

exit $LASTEXITCODE
