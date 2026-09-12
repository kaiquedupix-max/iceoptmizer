@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao por Maciota
set "LOG=%~dp0details.log"

echo aplicando otimizações...
ipconfig /flushdns
ipconfig /release
ipconfig /renew
Echo Abrindo DNSJumper!
start "" "%~dp0DnsJumper.exe"
echo Abrindo comando...



exit /b %errorlevel%
