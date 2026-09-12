@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao por Maciota
set "LOG=%~dp0details.log"

echo Arrumando...
sc config Audiosrv start= auto
sc start Audiosrv
sc config AudioEndpointBuilder start= auto
sc start AudioEndpointBuilder
net stop audiosrv
net start audiosrv


exit /b %errorlevel%
