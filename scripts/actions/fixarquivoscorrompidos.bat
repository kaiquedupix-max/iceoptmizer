@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao do Ice Optimizer
set "LOG=%~dp0details.log"

echo Arrumando...
echo Verificando arquivos do sistema...
sfc /scannow
echo Reparando imagem do Windows...
DISM /Online /Cleanup-Image /RestoreHealth


exit /b %errorlevel%
