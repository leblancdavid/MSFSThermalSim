sc.exe stop ThermalSim

timeout /t 5

sc.exe delete ThermalSim

timeout /t 5

rmdir /s /q "%ProgramFiles%\ThermalSim"