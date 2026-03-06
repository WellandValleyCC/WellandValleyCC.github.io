# Run all tests with the built‑in .NET coverage collector
dotnet test --collect:"XPlat Code Coverage"

# Generate a combined HTML coverage report
reportgenerator `
    -reports:**/coverage.cobertura.xml `
    -targetdir:coverage-report `
    -reporttypes:Html;HtmlSummary

# Open the report automatically (cross‑platform safe)
$reportPath = Join-Path -Path (Get-Location) -ChildPath "coverage-report/index.html"

if (Test-Path $reportPath) {
    Write-Host "Opening coverage report..." -ForegroundColor Green
    Start-Process $reportPath
} else {
    Write-Host "Coverage report not found at expected path: $reportPath" -ForegroundColor Yellow
}