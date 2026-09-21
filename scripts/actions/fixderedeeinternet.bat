@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao do Ice Optimizer
set "LOG=%~dp0details.log"

echo Arrumando...
ipconfig /flushdns
ipconfig /release
ipconfig /renew
netsh int ip reset
netsh winsock reset


exit /b %errorlevel%
