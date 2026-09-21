@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao do Ice Optimizer
set "LOG=%~dp0details.log"

echo Revertendo...
echo ===========================================
echo      RESTAURANDO CONFIGURACOES AO PADRAO
echo ===========================================
echo.

echo Restaurando MPO
reg delete "HKLM\SOFTWARE\Microsoft\Windows\Dwm" /v OverlayTestMode /f >nul 2>&1

echo Restaurando Game Mode
reg add "HKCU\Software\Microsoft\GameBar" /v AutoGameModeEnabled /t REG_DWORD /d 1 /f >nul
reg add "HKCU\Software\Microsoft\GameBar" /v AllowAutoGameMode /t REG_DWORD /d 1 /f >nul

echo Restaurando Game Bar
reg delete "HKLM\SOFTWARE\Policies\Microsoft\Windows\GameDVR" /v AllowGameDVR /f >nul 2>&1
reg delete "HKCU\System\GameConfigStore" /v GameDVR_Enabled /f >nul 2>&1

echo Restaurando Fullscreen Optimizations
reg delete "HKCU\System\GameConfigStore" /v GameDVR_FSEBehaviorMode /f >nul 2>&1
reg delete "HKCU\System\GameConfigStore" /v GameDVR_HonorUserFSEBehaviorMode /f >nul 2>&1

echo Restaurando Multimedia Scheduler
reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile" ^
/v SystemResponsiveness /t REG_DWORD /d 20 /f >nul

reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games" ^
/v "GPU Priority" /t REG_DWORD /d 8 /f >nul

reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games" ^
/v Priority /t REG_DWORD /d 2 /f >nul

reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games" ^
/v "Scheduling Category" /t REG_SZ /d Medium /f >nul

reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games" ^
/v "SFIO Priority" /t REG_SZ /d Normal /f >nul

echo Reativando NVIDIA Telemetry (caso exista)
sc config NvTelemetryContainer start= demand >nul 2>&1

echo.
echo ===========================================
echo Etapa encerrada. Verifique as mensagens acima.
echo Reinicie o computador.
echo ===========================================



exit /b %errorlevel%
