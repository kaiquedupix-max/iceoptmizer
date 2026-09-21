@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao por Ice Optimizer
set "LOG=%~dp0details.log"

echo Aumentando prioridade do Resident Evil Requiem...
reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\RustClient.exe" /f
reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\RustClient.exe\PerfOptions" /f
reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\RustClient.exe\PerfOptions" /v CpuPriorityClass /t REG_DWORD /d 3 /f


exit /b %errorlevel%
