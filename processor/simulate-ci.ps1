# Set working directory
Set-Location "C:\repos\wvcc\WellandValleyCC.github.io\processor"

# Timestamp for log file
$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$logPath = "logs\simulate-ci-$timestamp.log"
New-Item -ItemType File -Path $logPath -Force | Out-Null

# Ensure logs folder exists
if (-not (Test-Path "logs")) {
    New-Item -ItemType Directory -Path "logs" | Out-Null
}

# Helper to log and echo
function Log {
    param([string]$message)
    $message | Tee-Object -FilePath $logPath -Append
}

Log "Starting CI simulation at $timestamp"

# Step 1: Restore and build
Log "`nRestoring and building solution..."
dotnet restore .\ClubProcessor.sln | Tee-Object -FilePath $logPath -Append
dotnet build .\ClubProcessor.sln --configuration Release | Tee-Object -FilePath $logPath -Append

# Step 2: Run tests
Log "`nRunning unit tests..."
dotnet test .\ClubProcessor.Tests\ClubProcessor.Tests.csproj --no-build --verbosity normal | Tee-Object -FilePath $logPath -Append

# Step 3: Simulate file trigger
Log "`nSimulating file trigger..."
$triggerFile = Get-ChildItem "..\data\" | Where-Object { $_.Name -match "competitors_.*\.csv|.*Club Events\.xlsx" } | Select-Object -First 1

if ($null -eq $triggerFile) {
    Log "No trigger file found in ../data/"
} else {
    $filePath = "..\data\$($triggerFile.Name)"
    Log "Detected trigger file: $filePath"

    if ($triggerFile.Name -like "competitors_*") {
        Log "Running ClubProcessor in 'competitors' mode..."
        dotnet run --project .\ClubProcessor\ClubProcessor.csproj -- --mode competitors --file $filePath | Tee-Object -FilePath $logPath -Append
    } elseif ($triggerFile.Name -like "*Club Events.xlsx") {
        Log "Running ClubProcessor in 'events' mode..."
        dotnet run --project .\ClubProcessor\ClubProcessor.csproj -- --mode events --file $filePath | Tee-Object -FilePath $logPath -Append
    } else {
        Log "Unsupported file format: $($triggerFile.Name)"
    }
}

Log "`nCI simulation complete. Log saved to $logPath"
