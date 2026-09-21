@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao do Ice Optimizer
set "LOG=%~dp0details.log"

echo Removendo Windows Messaging...

powershell -Command "Get-AppxPackage *messaging* | Remove-AppxPackage"

echo Etapa encerrada. Verifique as mensagens acima.



exit /b %errorlevel%
