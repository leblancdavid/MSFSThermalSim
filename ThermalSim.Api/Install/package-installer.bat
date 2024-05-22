rmdir /S /Q "..\..\Installer"

mkdir "..\..\Installer"

xcopy /S /K /D /H /Y "..\bin\Release\net8.0-windows\" "..\..\Installer\ThermalSim\"

xcopy /Y "install.bat" "..\..\Installer\"
xcopy /Y "uninstall.bat" "..\..\Installer\"
xcopy /Y "dotnet-install.ps1" "..\..\Installer\"