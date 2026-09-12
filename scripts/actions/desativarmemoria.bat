@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao por Maciota
set "LOG=%~dp0details.log"

powershell -Command "Disable-MMAgent -MemoryCompression"



exit /b %errorlevel%
