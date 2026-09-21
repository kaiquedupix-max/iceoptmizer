@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao do Ice Optimizer
set "LOG=%~dp0details.log"

echo Ativando Modo de Jogo...
reg add "HKCU\Software\Microsoft\GameBar" /v AutoGameModeEnabled /t REG_DWORD /d 1 /f
reg add "HKCU\Software\Microsoft\GameBar" /v AllowAutoGameMode /t REG_DWORD /d 1 /f



exit /b %errorlevel%
