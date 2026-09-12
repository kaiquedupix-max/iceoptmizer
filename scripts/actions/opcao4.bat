@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao por Maciota
set "LOG=%~dp0details.log"

echo Desativando serviços inuteis...
sc stop WerSvc
sc config WerSvc start= disabled
sc stop DiagTrack
sc config DiagTrack start= disabled
sc stop dmwappushservice
sc config dmwappushservice start= disabled
sc stop WbioSrvc
sc config WbioSrvc start= disabled
sc stop Spooler
sc config Spooler start= disabled



exit /b %errorlevel%
