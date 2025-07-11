#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Test script to validate the solution builds and tests run successfully.

.DESCRIPTION
    This script mimics the GitHub Actions workflow steps to help debug
    build and test issues locally before pushing to GitHub.

.PARAMETER Configuration
    The build configuration to use (Debug or Release). Default is Debug.

.PARAMETER Platform
    The platform to build for (Any CPU or x86). Default is "Any CPU".

.EXAMPLE
    .\test-local-build.ps1
    .\test-local-build.ps1 -Configuration Release -Platform x86
#>

param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",

    [ValidateSet("Any CPU", "x86")]
    [string]$Platform = "Any CPU"
)

$ErrorActionPreference = "Stop"
$SolutionFile = "WinformsBoilerplate.sln"

Write-Host "=== Local Build Test ===" -ForegroundColor Green
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Platform: $Platform" -ForegroundColor Yellow
Write-Host ""

try {
    # Check if solution file exists
    if (-not (Test-Path $SolutionFile)) {
        throw "Solution file '$SolutionFile' not found in current directory"
    }

    Write-Host "Step 1: Restore dependencies" -ForegroundColor Cyan
    dotnet restore $SolutionFile --verbosity normal
    if ($LASTEXITCODE -ne 0) { throw "Restore failed" }
    Write-Host "✓ Restore completed successfully" -ForegroundColor Green
    Write-Host ""

    Write-Host "Step 2: Build solution" -ForegroundColor Cyan
    dotnet build $SolutionFile --no-restore --configuration $Configuration /p:Platform=$Platform --verbosity normal
    if ($LASTEXITCODE -ne 0) { throw "Build failed" }
    Write-Host "✓ Build completed successfully" -ForegroundColor Green
    Write-Host ""

    Write-Host "Step 3: Run tests" -ForegroundColor Cyan
    dotnet test $SolutionFile --configuration $Configuration /p:Platform=$Platform --verbosity normal --logger console --no-restore
    if ($LASTEXITCODE -ne 0) { throw "Tests failed" }
    Write-Host "✓ Tests completed successfully" -ForegroundColor Green
    Write-Host ""

    Write-Host "=== All steps completed successfully! ===" -ForegroundColor Green
    Write-Host "Your code is ready for GitHub Actions." -ForegroundColor Yellow

} catch {
    Write-Host ""
    Write-Host "=== Build/Test Failed ===" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Common solutions:" -ForegroundColor Yellow
    Write-Host "1. Make sure you're in the root directory with the .sln file"
    Write-Host "2. Check that all project references are correct"
    Write-Host "3. Ensure all NuGet packages are properly configured"
    Write-Host "4. Verify test projects have proper test framework references"
    Write-Host "5. Check for compilation errors in individual projects"
    Write-Host "6. Try running with --no-build removed: dotnet test without --no-build"
    Write-Host "7. Check assembly binding issues with fusion logs"
    Write-Host ""
    Write-Host "For assembly loading issues, try:" -ForegroundColor Cyan
    Write-Host "  dotnet test --configuration $Configuration /p:Platform=`"$Platform`" --verbosity diagnostic"
    Write-Host ""
    exit 1
}
