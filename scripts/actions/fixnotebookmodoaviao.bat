@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao por Maciota
set "LOG=%~dp0details.log"

echo Arrumando...
sc config RmSvc start= auto
sc start RmSvc
sc config WlanSvc start= auto
sc start WlanSvc


exit /b %errorlevel%
