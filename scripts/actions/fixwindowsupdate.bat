@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao do Ice Optimizer
set "LOG=%~dp0details.log"

echo Arrumando...
net stop wuauserv
net stop bits
net stop cryptsvc
ren %systemroot%\SoftwareDistribution SoftwareDistribution.old
ren %systemroot%\System32\catroot2 catroot2.old
net start wuauserv
net start bits
net start cryptsvc


exit /b %errorlevel%
