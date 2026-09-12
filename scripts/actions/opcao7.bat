@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao por Maciota
set "LOG=%~dp0details.log"

echo Desativando Relatórios de Erro do windows...
reg add "HKLM\SOFTWARE\Microsoft\Windows\Windows Error Reporting" /v Disabled /t REG_DWORD /d 1 /f
sc stop WerSvc >nul 2>&1
sc config WerSvc start=disabled >nul 2>&1



exit /b %errorlevel%
