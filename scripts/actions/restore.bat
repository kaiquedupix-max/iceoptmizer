@echo off
chcp 65001 >nul
powershell.exe -NoProfile -Command "try { Checkpoint-Computer -Description 'ice optimizer' -RestorePointType MODIFY_SETTINGS -ErrorAction Stop } catch { Write-Error $_; exit 1 }"
if errorlevel 1 exit /b 1
if not exist "%~dp0Backup" mkdir "%~dp0Backup"
reg export "HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer" "%~dp0Backup\Explorer.reg" /y
if errorlevel 1 exit /b 1
reg export "HKCU\Control Panel\Desktop" "%~dp0Backup\Desktop.reg" /y
exit /b %errorlevel%
