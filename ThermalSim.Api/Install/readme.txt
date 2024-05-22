HOW TO INSTALL:
	1. Run the "install.bat" (as Administrator)
	2. Copy the "kingsasquatchdave-thermal-sim-panel" into your MSFS2020 Community folder

HOW TO UNINSTALL:
	1. Run the "uninstall.bat" (as Administrator)
	2. Delete the "kingsasquatchdave-thermal-sim-panel" from your MSFS2020 Community folder


INSTALLATION INFO AND TROUBLESHOOTING

The "install.bat" installs the base ThermalSim service. The service depends on the .NET windows
desktop runtime v8.0.5, which is should download first if it's not found. Once the runtime
environment is installed, it will copy files into Program Files\ThermalSim\. Then it
creates a windows service which runs the ThermalSim.Api.exe service from that directory. 

The service is configured to run automatically on start up. If you prefer to not have it 
run on startup everytime, you can go to "Services" and find the "ThermalSim" service, go
to "Properties" and set the startup type to "Manual".

If you have any issues with installation please report the issue at 
https://github.com/leblancdavid/MSFSThermalSim/issues


Thank you and happy thermalling!