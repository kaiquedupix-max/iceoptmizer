@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao por Maciota
set "LOG=%~dp0details.log"

echo Arrumando...
sc config bthserv start= auto
sc start bthserv


exit /b %errorlevel%
