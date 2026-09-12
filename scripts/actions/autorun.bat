@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao por Maciota
set "LOG=%~dp0details.log"

start "" "%~dp0Autoruns.exe"
echo Configurando Inicialização do Windows...



exit /b %errorlevel%
