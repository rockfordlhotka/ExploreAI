# Simple PowerShell script to test the chat functionality
Write-Host "Testing chat functionality..." -ForegroundColor Yellow

# Test if we can at least call the chat command without 404 errors
Write-Host "Testing basic chat invocation..." -ForegroundColor Cyan
try {
    # Try to run the chat command with a simple timeout
    $pinfo = New-Object System.Diagnostics.ProcessStartInfo
    $pinfo.FileName = "dotnet"
    $pinfo.Arguments = "run --project ExploreAi -- chat --db ExploreAi.db"
    $pinfo.UseShellExecute = $false
    $pinfo.RedirectStandardOutput = $true
    $pinfo.RedirectStandardError = $true
    $pinfo.RedirectStandardInput = $true
    $pinfo.WorkingDirectory = "c:\dev\xebia\ExploreAI"

    $process = New-Object System.Diagnostics.Process
    $process.StartInfo = $pinfo
    $process.Start() | Out-Null

    # Send input to exit immediately
    $process.StandardInput.WriteLine("exit")
    $process.StandardInput.Close()

    # Wait for completion with timeout
    $process.WaitForExit(10000)

    $output = $process.StandardOutput.ReadToEnd()
    $errorOutput = $process.StandardError.ReadToEnd()

    Write-Host "Output:" -ForegroundColor Green
    Write-Host $output

    if ($errorOutput) {
        Write-Host "Error:" -ForegroundColor Red
        Write-Host $errorOutput
    }

    Write-Host "Exit Code: $($process.ExitCode)" -ForegroundColor Cyan

    if ($process.ExitCode -eq 0) {
        Write-Host "SUCCESS: Chat command completed without errors!" -ForegroundColor Green
    }
    else {
        Write-Host "WARNING: Chat command exited with code $($process.ExitCode)" -ForegroundColor Yellow
    }

}
catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
}
