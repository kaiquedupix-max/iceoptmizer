@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao do Ice Optimizer
set "LOG=%~dp0details.log"

echo Removendo Solitaire e Jogos Casuais...
powershell -Command "Get-AppxPackage *solitaire* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *xboxGamingOverlay* | Remove-AppxPackage"

echo Etapa encerrada. Verifique as mensagens acima.



exit /b %errorlevel%
