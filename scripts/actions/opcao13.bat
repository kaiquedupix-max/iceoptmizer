@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao por Maciota
set "LOG=%~dp0details.log"

echo Desativando Cortana...
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Windows Search" /v "AllowCortana" /t REG_DWORD /d 0 /f
powershell -Command "Get-AppxPackage *Microsoft.Windows.Cortana* | Remove-AppxPackage -ErrorAction SilentlyContinue"



exit /b %errorlevel%
