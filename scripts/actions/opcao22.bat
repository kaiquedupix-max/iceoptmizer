@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao do Ice Optimizer
set "LOG=%~dp0details.log"

echo Abrindo ISLC...

if exist "%~dp0ISLC v1.0.4.6\Intelligent standby list cleaner ISLC.exe" (
    start "" "%~dp0ISLC v1.0.4.6\Intelligent standby list cleaner ISLC.exe"
) else (
    echo ERRO: ISLC nao encontrado.
    echo Verifique se a pasta ISLC v1.0.4.6 esta junto com o .bat.
)



exit /b %errorlevel%
