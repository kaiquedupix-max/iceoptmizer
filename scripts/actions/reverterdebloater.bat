@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao do Ice Optimizer
set "LOG=%~dp0details.log"

powershell -Command "Get-AppxProvisionedPackage -Online | ForEach-Object { Add-AppxPackage -DisableDevelopmentMode -Register \"$($_.InstallLocation)\AppxManifest.xml\" }; Get-AppxPackage -AllUsers | ForEach-Object { $m = \"$($_.InstallLocation)\AppxManifest.xml\"; if (Test-Path $m) { Add-AppxPackage -DisableDevelopmentMode -Register $m } }"



exit /b %errorlevel%
