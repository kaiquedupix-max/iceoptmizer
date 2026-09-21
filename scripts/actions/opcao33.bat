@echo off
chcp 65001 >nul
setlocal EnableExtensions EnableDelayedExpansion
rem ice optimizer - Interface e integracao do Ice Optimizer
set "LOG=%~dp0details.log"

echo Desativando Economia de Energia do PCI Express...

powercfg /setacvalueindex SCHEME_CURRENT SUB_PCIEXPRESS ASPM 0
powercfg /setdcvalueindex SCHEME_CURRENT SUB_PCIEXPRESS ASPM 0
powercfg /setactive SCHEME_CURRENT

echo.
echo PCI Express Link State Power Management desativado!
echo O PCIe agora priorizara desempenho em vez de economia de energia.



exit /b %errorlevel%
