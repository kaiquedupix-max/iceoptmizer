@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao por Maciota
set "LOG=%~dp0details.log"

echo Arrumando...
sc config WlanSvc start= auto
sc start WlanSvc
sc config Dhcp start= auto
sc start Dhcp
sc config NlaSvc start= auto
sc start NlaSvc
sc config Netman start= auto
sc start Netman
netsh winsock reset
netsh int ip reset


exit /b %errorlevel%
