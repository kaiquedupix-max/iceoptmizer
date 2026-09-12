@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao por Maciota
set "LOG=%~dp0details.log"

Removendo Clima...
powershell -Command "Get-AppxPackage *Weather* | Remove-AppxPackage"

echo Etapa encerrada. Verifique as mensagens acima.



exit /b %errorlevel%
