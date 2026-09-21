@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao por Ice Optimizer
set "LOG=%~dp0details.log"

echo Desativar EcoQoS..
echo Reduzindo recursos de economia de desempenho...

reg add "HKLM\SYSTEM\CurrentControlSet\Control\Power\PowerThrottling" /v PowerThrottlingOff /t REG_DWORD /d 1 /f

echo.
echo Politicas de Power Throttling desativadas.
echo OBS: O Windows nao possui uma chave global oficial
echo para simplesmente desligar EcoQoS em todos os processos.



exit /b %errorlevel%
