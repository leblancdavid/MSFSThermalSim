cd /d "%~dp0"

sc.exe stop ThermalSim
timeout /t 1
sc.exe delete ThermalSim
timeout /t 1

rmdir /s /q "%ProgramFiles%\ThermalSim"