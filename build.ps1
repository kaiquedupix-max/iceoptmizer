$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (!(Test-Path -LiteralPath $compiler)) { throw '.NET Framework 4.x necessario.' }
New-Item -ItemType Directory -Force dist | Out-Null
& $compiler /nologo /target:winexe /optimize+ /out:dist\ice-optimizer.exe /win32manifest:src\app.manifest /win32icon:assets\ice.ico /resource:assets\ice.ico,ice.ico /resource:assets\login-reference.jpg,login-reference.jpg /resource:assets\account-reference.jpg,account-reference.jpg /resource:catalog.json,catalog.json /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:System.Net.Http.dll /reference:System.Web.Extensions.dll /reference:System.Security.dll /reference:System.Management.dll src\*.cs
if ($LASTEXITCODE -ne 0) { throw 'Falha ao compilar.' }
Copy-Item -LiteralPath scripts -Destination dist -Recurse -Force
Copy-Item README.md,THIRD-PARTY-NOTICES.md -Destination dist -Force
Compress-Archive -Path dist\ice-optimizer.exe,dist\scripts,dist\README.md,dist\THIRD-PARTY-NOTICES.md -DestinationPath dist\ice-optimizer-offline.zip -Force
Write-Output 'Build concluido: dist\ice-optimizer.exe e dist\ice-optimizer-offline.zip'

