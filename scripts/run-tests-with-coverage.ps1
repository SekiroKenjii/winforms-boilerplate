#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Runs all unit tests with code coverage collection.

.DESCRIPTION
    This script executes all unit tests in the solution and collects
    code coverage data. It can optionally generate HTML reports.

.PARAMETER Configuration
    The build configuration to use (Debug or Release). Default is Debug.

.PARAMETER GenerateHtmlReport
    Generate an HTML coverage report. Requires ReportGenerator tool.

.PARAMETER OutputPath
    The output path for coverage reports. Default is ./coverage-reports

.EXAMPLE
    .\run-tests-with-coverage.ps1
    Runs tests with coverage collection

.EXAMPLE
    .\run-tests-with-coverage.ps1 -GenerateHtmlReport -OutputPath "./reports"
    Runs tests with coverage and generates HTML report
#>

param(
    [Parameter()]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",

    [Parameter()]
    [switch]$GenerateHtmlReport,

    [Parameter()]
    [string]$OutputPath = "./coverage-reports"
)

Write-Host "🧪 Running Unit Tests with Coverage for WinForms Boilerplate" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Output Path: $OutputPath" -ForegroundColor Yellow
Write-Host ""

# Ensure we're in the root directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$rootPath = Split-Path -Parent $scriptPath
Set-Location $rootPath

# Create output directory
if (!(Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath | Out-Null
}

# Build the solution first
Write-Host "🔨 Building solution..." -ForegroundColor Green
dotnet build --configuration $Configuration
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed. Exiting." -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "🏃‍♂️ Running tests with coverage..." -ForegroundColor Green

# Run tests with coverage collection
dotnet test --configuration $Configuration --no-build `
    --collect:"XPlat Code Coverage" `
    --results-directory $OutputPath `
    --settings coverlet.runsettings

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Tests failed. Check the output above for details." -ForegroundColor Red
    exit $LASTEXITCODE
}

# Generate HTML report if requested
if ($GenerateHtmlReport) {
    Write-Host ""
    Write-Host "📊 Generating HTML coverage report..." -ForegroundColor Green

    # Check if ReportGenerator is installed
    $reportGenerator = Get-Command "reportgenerator" -ErrorAction SilentlyContinue
    if ($null -eq $reportGenerator) {
        Write-Host "Installing ReportGenerator tool..." -ForegroundColor Yellow
        dotnet tool install --global dotnet-reportgenerator-globaltool
    }

    # Find coverage files
    $coverageFiles = Get-ChildItem -Path $OutputPath -Recurse -Filter "coverage.cobertura.xml" | Select-Object -ExpandProperty FullName

    if ($coverageFiles.Count -gt 0) {
        $coverageInput = $coverageFiles -join ";"
        $htmlOutputPath = Join-Path $OutputPath "html"

        reportgenerator -reports:$coverageInput -targetdir:$htmlOutputPath -reporttypes:Html

        if ($LASTEXITCODE -eq 0) {
            Write-Host "📈 HTML report generated at: $htmlOutputPath" -ForegroundColor Green
            $indexPath = Join-Path $htmlOutputPath "index.html"
            Write-Host "Open: $indexPath" -ForegroundColor Cyan
        }
    } else {
        Write-Host "⚠️  No coverage files found to generate HTML report." -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "✅ Coverage collection completed!" -ForegroundColor Green
Write-Host "📁 Results saved to: $OutputPath" -ForegroundColor Cyan
