$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot
$compiler = Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'
if (!(Test-Path -LiteralPath $compiler)) { throw '.NET Framework 4.x necessario.' }
New-Item -ItemType Directory -Force dist | Out-Null
$version = (Get-Content VERSION -Raw).Trim()
if ($version -notmatch '^\d+\.\d+\.\d+Remove-Item -LiteralPath dist\scripts -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -LiteralPath dist\ice-optimizer-offline.zip -Force -ErrorAction SilentlyContinue
& $compiler /nologo /target:winexe /optimize+ /out:dist\ice-optimizer.exe /win32manifest:src\app.manifest /win32icon:assets\ice.ico /resource:assets\ice.ico,ice.ico /resource:assets\login-reference.jpg,login-reference.jpg /resource:assets\account-reference.jpg,account-reference.jpg /resource:assets\home-reference.png,home-reference.png /resource:assets\ice-click.wav,ice-click.wav /resource:catalog.json,catalog.json /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:System.Net.Http.dll /reference:System.Web.Extensions.dll /reference:System.Security.dll /reference:System.Management.dll src\*.cs dist\AssemblyInfo.g.cs
if ($LASTEXITCODE -ne 0) { throw 'Falha ao compilar.' }
Copy-Item README.md,THIRD-PARTY-NOTICES.md -Destination dist -Force
Write-Output "Build concluido: dist\\ice-optimizer.exe (versao $version; downloads sob demanda com SHA-256)"

) { throw 'VERSION invalida.' }
$assemblyVersion = "$version.0"
$assemblyInfo = @"
using System.Reflection;
[assembly: AssemblyTitle("Ice Optimizer")]
[assembly: AssemblyDescription("Otimizador de desempenho e manutencao para Windows")]
[assembly: AssemblyCompany("Ice Optimizer")]
[assembly: AssemblyProduct("Ice Optimizer")]
[assembly: AssemblyCopyright("Copyright (c) Ice Optimizer")]
[assembly: AssemblyVersion("$assemblyVersion")]
[assembly: AssemblyFileVersion("$assemblyVersion")]
[assembly: AssemblyInformationalVersion("$version")]
"@
[IO.File]::WriteAllText((Join-Path $PSScriptRoot 'dist\AssemblyInfo.g.cs'), $assemblyInfo, (New-Object Text.UTF8Encoding($false)))
Remove-Item -LiteralPath dist\scripts -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -LiteralPath dist\ice-optimizer-offline.zip -Force -ErrorAction SilentlyContinue
& $compiler /nologo /target:winexe /optimize+ /out:dist\ice-optimizer.exe /win32manifest:src\app.manifest /win32icon:assets\ice.ico /resource:assets\ice.ico,ice.ico /resource:assets\login-reference.jpg,login-reference.jpg /resource:assets\account-reference.jpg,account-reference.jpg /resource:assets\home-reference.png,home-reference.png /resource:assets\ice-click.wav,ice-click.wav /resource:catalog.json,catalog.json /reference:System.Windows.Forms.dll /reference:System.Drawing.dll /reference:System.Net.Http.dll /reference:System.Web.Extensions.dll /reference:System.Security.dll /reference:System.Management.dll src\*.cs
if ($LASTEXITCODE -ne 0) { throw 'Falha ao compilar.' }
Copy-Item README.md,THIRD-PARTY-NOTICES.md -Destination dist -Force
Write-Output 'Build concluido: dist\ice-optimizer.exe (scripts sincronizados online a cada abertura)'

