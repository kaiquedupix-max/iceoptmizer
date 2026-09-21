@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao do Ice Optimizer
set "LOG=%~dp0details.log"

echo Removendo Anúncios e sugestões...

powershell -command "Set-ItemProperty HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager SystemPaneSuggestionsEnabled 0"
powershell -command "Set-ItemProperty HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager 'SubscribedContent-338393Enabled' 0"
powershell -command "Set-ItemProperty HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager 'SubscribedContent-353694Enabled' 0"
powershell -command "Set-ItemProperty HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager 'SubscribedContent-353696Enabled' 0"
powershell -command "Set-ItemProperty HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager 'SubscribedContent-338389Enabled' 0"
powershell -command "Set-ItemProperty HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager 'SubscribedContent-310093Enabled' 0"
powershell -command "Set-ItemProperty HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager 'SubscribedContent-353698Enabled' 0"
powershell -command "Set-ItemProperty HKCU:\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager SoftLandingEnabled 0"

powershell -command "Set-ItemProperty HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced Start_TrackProgs 0"
powershell -command "Set-ItemProperty HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced Start_TrackDocs 0"

powershell -command "Set-ItemProperty HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer ShowRecent 0"
powershell -command "Set-ItemProperty HKCU:\Software\Microsoft\Windows\CurrentVersion\Explorer ShowRecommended 0"

echo Etapa encerrada. Verifique as mensagens acima.



exit /b %errorlevel%
