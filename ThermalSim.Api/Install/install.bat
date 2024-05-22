PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& {Start-Process PowerShell -ArgumentList '-NoProfile -ExecutionPolicy Bypass -File ""dotnet-install.ps1 -Architecture x64 -Runtime windowsdesktop -Version 8.0.5""' -Verb RunAs}"

mkdir "%ProgramFiles%\ThermalSim"

xcopy /S /K /D /H /Y "ThermalSim\" "%ProgramFiles%\ThermalSim\"

sc.exe create ThermalSim binpath="%ProgramFiles%\ThermalSim\ThermalSim.Api.exe" start= auto
sc.exe description ThermalSim "The ThermalSim service which connects to MSFS2020 SimConnect and runs the thermal simulation"
sc.exe start ThermalSim