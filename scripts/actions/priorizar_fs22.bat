@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao do Ice Optimizer
set "LOG=%~dp0details.log"

echo Aumentando prioridade do Farming Simulator 22...
reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\FarmingSimulator2022.exe" /f
reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\FarmingSimulator2022.exe\PerfOptions" /f
reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\FarmingSimulator2022.exe\PerfOptions" /v CpuPriorityClass /t REG_DWORD /d 3 /f


exit /b %errorlevel%
