@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao do Ice Optimizer
set "LOG=%~dp0details.log"

sc config XblAuthManager start=demand >nul 2>&1
sc config XblGameSave start=demand >nul 2>&1
sc config XboxNetApiSvc start=demand >nul 2>&1
sc config XboxGipSvc start=demand >nul 2>&1
powershell -Command "Get-AppxPackage -AllUsers Microsoft.XboxGamingOverlay | Foreach {Add-AppxPackage -DisableDevelopmentMode -Register '$($_.InstallLocation)\AppXManifest.xml'}"
reg add "HKCU\System\GameConfigStore" /v GameDVR_Enabled /t REG_DWORD /d 1 /f



exit /b %errorlevel%
