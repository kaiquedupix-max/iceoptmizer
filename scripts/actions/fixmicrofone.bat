@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao por Maciota
set "LOG=%~dp0details.log"

echo Arrumando...
call :step "Criando ponto de restauracao, se disponivel"
powershell -NoProfile -ExecutionPolicy Bypass -Command "try { Enable-ComputerRestore -Drive ($env:SystemDrive+'\\') -ErrorAction SilentlyContinue; Checkpoint-Computer -Description 'Antes do FIX Microfone' -RestorePointType 'MODIFY_SETTINGS' -ErrorAction SilentlyContinue } catch {}" >> "%LOG%" 2>&1

call :step "Fechando aplicativos que podem prender o microfone"
for %%P in (Teams.exe ms-teams.exe Zoom.exe Discord.exe Skype.exe obs64.exe obs32.exe) do taskkill /F /IM %%P >nul 2>&1

call :step "Ativando permissoes do microfone no Windows"
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\microphone" /v Value /t REG_SZ /d Allow /f >> "%LOG%" 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\microphone" /v Value /t REG_SZ /d Allow /f >> "%LOG%" 2>&1
reg add "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\microphone\NonPackaged" /v Value /t REG_SZ /d Allow /f >> "%LOG%" 2>&1
reg add "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\microphone\NonPackaged" /v Value /t REG_SZ /d Allow /f >> "%LOG%" 2>&1

call :step "Removendo politicas que podem bloquear o microfone"
reg delete "HKLM\SOFTWARE\Policies\Microsoft\Windows\AppPrivacy" /v LetAppsAccessMicrophone /f >> "%LOG%" 2>&1
reg delete "HKCU\SOFTWARE\Policies\Microsoft\Windows\AppPrivacy" /v LetAppsAccessMicrophone /f >> "%LOG%" 2>&1

call :step "Configurando e reiniciando os principais servicos de audio"
sc config AudioEndpointBuilder start= auto >> "%LOG%" 2>&1
sc config Audiosrv start= auto >> "%LOG%" 2>&1
sc config PlugPlay start= auto >> "%LOG%" 2>&1
net stop Audiosrv /y >> "%LOG%" 2>&1
net stop AudioEndpointBuilder /y >> "%LOG%" 2>&1
net start AudioEndpointBuilder >> "%LOG%" 2>&1
net start Audiosrv >> "%LOG%" 2>&1

call :step "Reativando dispositivos de audio e microfone"
powershell -NoProfile -ExecutionPolicy Bypass -Command "$devs=Get-PnpDevice -PresentOnly:$false -ErrorAction SilentlyContinue | Where-Object { $_.Class -in @('AudioEndpoint','MEDIA') -or $_.FriendlyName -match 'microphone|microfone|mic|audio|headset' }; foreach($d in $devs){ try{Enable-PnpDevice -InstanceId $d.InstanceId -Confirm:$false -ErrorAction SilentlyContinue}catch{} }; $devs | Format-Table Status,Class,FriendlyName,InstanceId -AutoSize" >> "%LOG%" 2>&1

call :step "Forcando nova deteccao de hardware de audio"
pnputil /scan-devices >> "%LOG%" 2>&1

timeout /t 3 /nobreak >nul

call :step "Reiniciando endpoints e dispositivos de audio"
powershell -NoProfile -ExecutionPolicy Bypass -Command "$devs=Get-PnpDevice -ErrorAction SilentlyContinue | Where-Object { $_.Class -in @('AudioEndpoint','MEDIA') -and $_.Status -ne 'Unknown' }; foreach($d in $devs){ try{Disable-PnpDevice -InstanceId $d.InstanceId -Confirm:$false -ErrorAction SilentlyContinue; Start-Sleep -Milliseconds 500; Enable-PnpDevice -InstanceId $d.InstanceId -Confirm:$false -ErrorAction SilentlyContinue}catch{}}" >> "%LOG%" 2>&1

call :step "Limpando cache de deteccao de audio e reiniciando servicos"
net stop Audiosrv /y >> "%LOG%" 2>&1
net stop AudioEndpointBuilder /y >> "%LOG%" 2>&1
timeout /t 2 /nobreak >nul
net start AudioEndpointBuilder >> "%LOG%" 2>&1
net start Audiosrv >> "%LOG%" 2>&1

call :step "Verificando integridade do Windows (DISM + SFC)"
DISM /Online /Cleanup-Image /ScanHealth >> "%LOG%" 2>&1
DISM /Online /Cleanup-Image /RestoreHealth >> "%LOG%" 2>&1
sfc /scannow >> "%LOG%" 2>&1

call :step "Tentando iniciar Windows Update para drivers de audio"
sc start wuauserv >> "%LOG%" 2>&1
sc start bits >> "%LOG%" 2>&1
UsoClient StartScan >> "%LOG%" 2>&1
UsoClient StartDownload >> "%LOG%" 2>&1

call :step "Abrindo painel de som para conferir o dispositivo padrao"
start ms-settings:sound

call :step "Listando status final dos dispositivos de audio"
powershell -NoProfile -ExecutionPolicy Bypass -Command "Get-PnpDevice -ErrorAction SilentlyContinue | Where-Object { $_.Class -in @('AudioEndpoint','MEDIA') -or $_.FriendlyName -match 'microphone|microfone|mic|audio|headset' } | Format-Table Status,Class,FriendlyName,Problem,InstanceId -AutoSize" >> "%LOG%" 2>&1

echo.
echo ============================================================
echo FIX DO MICROFONE FINALIZADO
echo ============================================================
echo.
echo Confira em Configuracoes ^> Sistema ^> Som ^> Entrada:
echo - se o microfone correto esta selecionado;
echo - se o volume de entrada nao esta em 0;
echo - se o dispositivo nao esta desabilitado.
echo.
echo Se for USB/P2, desconecte e conecte novamente e reinicie o PC.
echo.
echo Log salvo em:
echo %LOG%
echo.


exit /b %errorlevel%

:step
echo [*] %~1
exit /b 0
