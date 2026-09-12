@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao por Maciota
set "LOG=%~dp0details.log"

echo Desativar Economia do Adaptador de Rede..
echo Desativando Economia de Energia dos Adaptadores de Rede...

powershell -NoProfile -ExecutionPolicy Bypass -Command "Get-NetAdapter -Physical -ErrorAction SilentlyContinue | ForEach-Object { Disable-NetAdapterPowerManagement -Name $_.Name -ErrorAction SilentlyContinue }"

echo.
echo Economia de energia dos adaptadores de rede desativada!
echo Alguns drivers podem nao oferecer suporte a esta configuracao.



exit /b %errorlevel%
