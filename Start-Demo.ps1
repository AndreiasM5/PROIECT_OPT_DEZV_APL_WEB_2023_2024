param([switch]$SkipBuild)
$ErrorActionPreference='Stop'
Set-Location -LiteralPath $PSScriptRoot
New-Item -ItemType Directory -Force .runtime,run-logs | Out-Null
$runtimeFile=Join-Path $PSScriptRoot '.runtime/runtime.json'
$dotnet=if(Test-Path $runtimeFile){(Get-Content $runtimeFile -Raw|ConvertFrom-Json).dotnet}else{(Get-Command dotnet -ErrorAction Stop).Source}
$node=(Get-Command node -ErrorAction Stop).Source
foreach($port in 5156,4300){
    $client=[Net.Sockets.TcpClient]::new()
    try{$client.Connect('127.0.0.1',$port);throw "Port $port is already in use. Stop the existing demo first."}catch [Net.Sockets.SocketException]{}finally{$client.Dispose()}
}
if(-not(Test-Path Frontend/node_modules)){Push-Location Frontend;try{npm.cmd ci;if($LASTEXITCODE -ne 0){throw 'npm ci failed'}}finally{Pop-Location}}
if(-not $SkipBuild){
    & $dotnet build Backend/Backend.csproj
    if($LASTEXITCODE -ne 0){throw 'Backend build failed'}
    Push-Location Frontend
    try{npm.cmd test;if($LASTEXITCODE -ne 0){throw 'Frontend tests failed'};npm.cmd run build;if($LASTEXITCODE -ne 0){throw 'Frontend build failed'}}finally{Pop-Location}
}
$env:ASPNETCORE_ENVIRONMENT='Development'
$env:ASPNETCORE_URLS='http://127.0.0.1:5156'
$backend=Start-Process -FilePath $dotnet -ArgumentList 'bin/Debug/net10.0/Backend.dll' -WorkingDirectory (Join-Path $PSScriptRoot 'Backend') -WindowStyle Hidden -RedirectStandardOutput (Join-Path $PSScriptRoot 'run-logs/backend.log') -RedirectStandardError (Join-Path $PSScriptRoot 'run-logs/backend-error.log') -PassThru
$processes=@{backend=$backend.Id;frontend=$null}
try{
    $ready=$false
    for($attempt=0;$attempt -lt 60;$attempt++){
        if($backend.HasExited){throw 'Backend exited. Check run-logs/backend.log.'}
        try{$null=Invoke-RestMethod http://127.0.0.1:5156/api/health;$ready=$true;break}catch{Start-Sleep -Milliseconds 500}
    }
    if(-not $ready){throw 'Backend did not become ready'}
    $frontend=Start-Process -FilePath $node -ArgumentList 'node_modules/@angular/cli/bin/ng.js','serve','--host','127.0.0.1','--port','4300','--proxy-config','proxy.conf.json' -WorkingDirectory (Join-Path $PSScriptRoot 'Frontend') -WindowStyle Hidden -RedirectStandardOutput (Join-Path $PSScriptRoot 'run-logs/frontend.log') -RedirectStandardError (Join-Path $PSScriptRoot 'run-logs/frontend-error.log') -PassThru
    $processes.frontend=$frontend.Id
    $processes|ConvertTo-Json|Set-Content .runtime/processes.json
    $ready=$false
    for($attempt=0;$attempt -lt 60;$attempt++){
        if($frontend.HasExited){throw 'Frontend exited. Check run-logs/frontend-error.log.'}
        try{$null=Invoke-WebRequest http://127.0.0.1:4300 -UseBasicParsing;$ready=$true;break}catch{Start-Sleep -Milliseconds 500}
    }
    if(-not $ready){throw 'Frontend did not become ready'}
    Write-Host 'Angular: http://127.0.0.1:4300'
    Write-Host 'Swagger: http://127.0.0.1:5156/swagger/index.html'
}catch{foreach($value in $processes.Values){if($value){Stop-Process -Id $value -ErrorAction SilentlyContinue}};throw}
