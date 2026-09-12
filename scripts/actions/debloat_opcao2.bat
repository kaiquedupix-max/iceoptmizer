@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao por Maciota
set "LOG=%~dp0details.log"

echo Removendo todos programas inúteis do Windows...

powershell -Command "Get-AppxPackage *Microsoft.Windows.Cortana* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *officehub* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *store* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *xbox* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *photos* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *phone* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *people* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *music* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *messaging* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *maps* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *groove* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *getstarted* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *calendar* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *calculator* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *alarms* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *3dbuilder* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *camera* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *news* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *onedrive* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *FeedbackHub* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *windowscommunicationsapps* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *QuickAssist* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *todos* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *solitaire* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *xboxGamingOverlay* | Remove-AppxPackage"
powershell -Command "Get-AppxPackage *Weather* | Remove-AppxPackage"

reg add "HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced" /v ShowCopilotButton /t REG_DWORD /d 0 /f
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Windows Copilot" /v TurnOffWindowsCopilot /t REG_DWORD /d 1 /f
reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer" /v HideCopilotButton /f
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Windows Search" /v "AllowCortana" /t REG_DWORD /d 0 /f
reg add "HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager" /v "SystemPaneSuggestionsEnabled" /t REG_DWORD /d 0 /f
echo Etapa encerrada. Verifique as mensagens acima.



exit /b %errorlevel%
