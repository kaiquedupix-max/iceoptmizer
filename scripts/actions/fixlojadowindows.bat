@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao por Maciota
set "LOG=%~dp0details.log"

echo Arrumando...
sc config BITS start= auto
sc start BITS
powershell -command "Get-AppxPackage -AllUsers | Foreach {Add-AppxPackage -DisableDevelopmentMode -Register `$($_.InstallLocation)\AppxManifest.xml}"


exit /b %errorlevel%
