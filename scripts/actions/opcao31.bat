@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao do Ice Optimizer
set "LOG=%~dp0details.log"

echo Desativando Suspensao Seletiva de USB...
powercfg /setacvalueindex SCHEME_CURRENT SUB_USB USBSELECTIVE 0
powercfg /setdcvalueindex SCHEME_CURRENT SUB_USB USBSELECTIVE 0
powercfg /setactive SCHEME_CURRENT
echo.
echo Suspensao Seletiva de USB desativada!
echo Dispositivos USB nao serao suspensos para economizar energia.



exit /b %errorlevel%
