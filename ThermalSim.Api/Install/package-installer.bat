rmdir /S /Q "..\..\Installer"

mkdir "..\..\Installer"

xcopy /S /C /D /H /Y "..\bin\Release\net8.0-windows\" "..\..\Installer\ThermalSim\"

xcopy /Y "install.bat" "..\..\Installer\"
xcopy /Y "uninstall.bat" "..\..\Installer\"
xcopy /Y "dotnet-install.ps1" "..\..\Installer\"
xcopy /Y "readme.txt" "..\..\Installer\"


mkdir "..\..\Installer\kingsasquatchdave-thermal-sim-panel"
xcopy /S /C /D /H /Y "..\..\Client\Addons\InGamePanel\Output\Packages\thermal-sim-panel\" "..\..\Installer\kingsasquatchdave-thermal-sim-panel\"

::del "..\..\kingsasquatchdave-thermal-sim.zip"
::powershell.exe "Compress-Archive" -Path "..\..\Installer\" -DestinationPath "..\..\kingsasquatchdave-thermal-sim.zip"