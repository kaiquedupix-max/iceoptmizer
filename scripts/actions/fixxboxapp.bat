@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao por Maciota
set "LOG=%~dp0details.log"

echo Arrumando...
net stop XboxGipSvc
net stop XblAuthManager
net stop XblGameSave
net stop XboxNetApiSvc
powershell -command "Get-AppxPackage *Xbox* | Foreach {Add-AppxPackage -DisableDevelopmentMode -Register `$($_.InstallLocation)\AppxManifest.xml}"
net start XboxGipSvc
net start XblAuthManager
net start XblGameSave
net start XboxNetApiSvc


exit /b %errorlevel%
