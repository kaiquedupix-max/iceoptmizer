@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao por Maciota
set "LOG=%~dp0details.log"

echo Removendo Windows Getstarted...

powershell -Command "Get-AppxPackage *getstarted* | Remove-AppxPackage"

echo Etapa encerrada. Verifique as mensagens acima.



exit /b %errorlevel%
