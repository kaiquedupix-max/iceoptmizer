@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao do Ice Optimizer
set "LOG=%~dp0details.log"

echo Removendo Copilot...

powershell -command "Get-AppxPackage *Copilot* | Remove-AppxPackage"
powershell -command "Get-AppxPackage -AllUsers *Copilot* | Remove-AppxPackage"
powershell -command "Get-AppxProvisionedPackage -Online | Where-Object {$_.DisplayName -like '*Copilot*'} | Remove-AppxProvisionedPackage -Online"

echo Etapa encerrada. Verifique as mensagens acima.



exit /b %errorlevel%
