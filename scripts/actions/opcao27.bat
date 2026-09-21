@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao do Ice Optimizer
set "LOG=%~dp0details.log"

echo Limpando cache de atualizações e pastas temporárias do windows...
del /s /f /q "%windir%\Temp\*.*" 2>nul
for /d %%x in ("%windir%\Temp\*") do rd /s /q "%%x" 2>nul
del /s /f /q "%temp%\*.*" 2>nul
for /d %%x in ("%temp%\*") do rd /s /q "%%x" 2>nul
del /s /f /q "%APPDATA%\Microsoft\Windows\Recent\*.*" 2>nul
ipconfig /flushdns >nul
net stop wuauserv >nul 2>&1
del /s /f /q "%windir%\SoftwareDistribution\Download\*.*" 2>nul
net start wuauserv >nul 2>&1
PowerShell.exe -NoProfile -Command Clear-RecycleBin -Force 2>nul
echo Etapa encerrada. Verifique as mensagens acima.



exit /b %errorlevel%
