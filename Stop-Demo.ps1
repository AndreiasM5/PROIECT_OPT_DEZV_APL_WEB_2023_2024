$ErrorActionPreference='Stop'
$file=Join-Path $PSScriptRoot '.runtime/processes.json'
if(-not(Test-Path $file)){Write-Host 'No demo process file found.';return}
$processes=Get-Content $file -Raw | ConvertFrom-Json
foreach($kind in 'frontend','backend'){
    $processId=$processes.$kind
    if(-not $processId){continue}
    $process=Get-CimInstance Win32_Process -Filter "ProcessId=$processId"
    if(-not $process){continue}
    $expected=if($kind -eq 'backend'){'Backend.dll'}else{'@angular/cli/bin/ng.js'}
    if($process.CommandLine -and $process.CommandLine.Contains($expected)){
        Stop-Process -Id $processId
        Write-Host "Stopped $kind ($processId)."
    }else{Write-Warning "Skipped process ${processId}: command line does not match the demo."}
}
Remove-Item -LiteralPath $file
Write-Host 'Demo stopped. SQLite data is preserved in Backend/data/.'
