@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao do Ice Optimizer
set "LOG=%~dp0details.log"

echo Verificando arquivos e integridade do Windows...
DISM /Online /Cleanup-Image /RestoreHealth
sfc /scannow



exit /b %errorlevel%
