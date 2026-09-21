@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao do Ice Optimizer
set "LOG=%~dp0details.log"

echo Desativando indexação de pesquisa (menu iniciar)...
net stop "Windows Search" >nul 2>&1
sc config "WSearch" start= disabled >nul 2>&1



exit /b %errorlevel%
