@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao do Ice Optimizer
set "LOG=%~dp0details.log"

echo Arrumando...
call :step "Fechando aplicativos que podem prender a camera"
for %%P in (Teams.exe ms-teams.exe Zoom.exe Discord.exe Skype.exe obs64.exe obs32.exe Camera.exe WindowsCamera.exe) do taskkill /F /IM %%P >nul 2>&1

call :step "Ativando permissoes de camera no Windows"
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam" /v Value /t REG_SZ /d Allow /f >> "%LOG%" 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam" /v Value /t REG_SZ /d Allow /f >> "%LOG%" 2>&1
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam\NonPackaged" /v Value /t REG_SZ /d Allow /f >> "%LOG%" 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\webcam\NonPackaged" /v Value /t REG_SZ /d Allow /f >> "%LOG%" 2>&1

call :step "Removendo politicas que podem bloquear a camera"
reg delete "HKLM\SOFTWARE\Policies\Microsoft\Camera" /v AllowCamera /f >> "%LOG%" 2>&1
reg delete "HKCU\SOFTWARE\Policies\Microsoft\Camera" /v AllowCamera /f >> "%LOG%" 2>&1
reg delete "HKLM\SOFTWARE\Policies\Microsoft\Windows\AppPrivacy" /v LetAppsAccessCamera /f >> "%LOG%" 2>&1
reg delete "HKCU\SOFTWARE\Policies\Microsoft\Windows\AppPrivacy" /v LetAppsAccessCamera /f >> "%LOG%" 2>&1

call :step "Reiniciando servicos relacionados a dispositivos e captura"
for %%S in (PlugPlay DeviceInstall DsmSvc FrameServer FrameServerMonitor) do (
    sc query %%S >nul 2>&1 && (
        sc config %%S start= demand >> "%LOG%" 2>&1
        net stop %%S /y >> "%LOG%" 2>&1
        net start %%S >> "%LOG%" 2>&1
    )
)

call :step "Reiniciando servicos de usuario relacionados a camera"
powershell -NoProfile -ExecutionPolicy Bypass -Command "$svcs=Get-Service | Where-Object { $_.Name -like 'CaptureService_*' -or $_.Name -like 'cbdhsvc_*' }; foreach($s in $svcs){ try{Restart-Service -Name $s.Name -Force -ErrorAction SilentlyContinue}catch{}}" >> "%LOG%" 2>&1

call :step "Reativando dispositivos de Camera/Image via Plug and Play"
powershell -NoProfile -ExecutionPolicy Bypass -Command "$devs=Get-PnpDevice -PresentOnly:$false -ErrorAction SilentlyContinue | Where-Object { $_.Class -in @('Camera','Image') -or $_.FriendlyName -match 'camera|webcam|integrated cam|usb video' }; foreach($d in $devs){ try{Enable-PnpDevice -InstanceId $d.InstanceId -Confirm:$false -ErrorAction SilentlyContinue}catch{} }; $devs | Format-Table Status,Class,FriendlyName,InstanceId -AutoSize" >> "%LOG%" 2>&1

call :step "Forcando nova deteccao de hardware"
pnputil /scan-devices >> "%LOG%" 2>&1

timeout /t 3 /nobreak >nul

call :step "Tentando reiniciar os dispositivos de camera"
powershell -NoProfile -ExecutionPolicy Bypass -Command "$devs=Get-PnpDevice -ErrorAction SilentlyContinue | Where-Object { $_.Class -in @('Camera','Image') -or $_.FriendlyName -match 'camera|webcam|integrated cam|usb video' }; foreach($d in $devs){ try{Disable-PnpDevice -InstanceId $d.InstanceId -Confirm:$false -ErrorAction SilentlyContinue; Start-Sleep -Milliseconds 700; Enable-PnpDevice -InstanceId $d.InstanceId -Confirm:$false -ErrorAction SilentlyContinue}catch{}}" >> "%LOG%" 2>&1

call :step "Verificando arquivos do Windows (DISM + SFC)"
DISM /Online /Cleanup-Image /ScanHealth >> "%LOG%" 2>&1
DISM /Online /Cleanup-Image /RestoreHealth >> "%LOG%" 2>&1
sfc /scannow >> "%LOG%" 2>&1

call :step "Atualizando componentes da Microsoft Store / Camera, quando possivel"
powershell -NoProfile -ExecutionPolicy Bypass -Command "if(Get-Command winget -ErrorAction SilentlyContinue){ winget source update --disable-interactivity | Out-Null; winget upgrade --id Microsoft.WindowsCamera --accept-source-agreements --accept-package-agreements --silent 2>$null }" >> "%LOG%" 2>&1

call :step "Re-registrando o aplicativo Camera do Windows"
powershell -NoProfile -ExecutionPolicy Bypass -Command "Get-AppxPackage -AllUsers *WindowsCamera* -ErrorAction SilentlyContinue | ForEach-Object { try { Add-AppxPackage -DisableDevelopmentMode -Register ($_.InstallLocation + '\\AppXManifest.xml') -ErrorAction SilentlyContinue } catch {} }" >> "%LOG%" 2>&1

call :step "Tentando iniciar o Windows Update para drivers"
sc start wuauserv >> "%LOG%" 2>&1
sc start bits >> "%LOG%" 2>&1
UsoClient StartScan >> "%LOG%" 2>&1
UsoClient StartDownload >> "%LOG%" 2>&1

call :step "Listando status final da camera"
powershell -NoProfile -ExecutionPolicy Bypass -Command "Get-PnpDevice -ErrorAction SilentlyContinue | Where-Object { $_.Class -in @('Camera','Image') -or $_.FriendlyName -match 'camera|webcam|usb video' } | Format-Table Status,Class,FriendlyName,Problem,InstanceId -AutoSize" >> "%LOG%" 2>&1

echo.
echo ============================================================
echo FIX DA CAMERA FINALIZADO
echo ============================================================
echo.
echo 1. Desconecte e conecte a webcam novamente, se for USB.
echo 2. Reinicie o PC para aplicar tudo.
echo 3. Depois teste em Configuracoes ^> Bluetooth e dispositivos ^> Cameras
echo    ou abra o aplicativo Camera.
echo.
echo Log salvo em:
echo %LOG%
echo.


exit /b %errorlevel%

:step
echo [*] %~1
exit /b 0
