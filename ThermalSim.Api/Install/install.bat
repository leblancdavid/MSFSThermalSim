cd /d "%~dp0"

if not exist "%ProgramFiles%\dotnet\shared\Microsoft.WindowsDesktop.App\8.0.5\" (
    curl "https://download.visualstudio.microsoft.com/download/pr/e2ced2b3-e1a5-401a-bcc9-6689e4eea673/93f77de4a39a219d775b403b7ef0cf58/aspnetcore-runtime-8.0.5-win-x64.exe" -o "aspnetcore-runtime-install.exe"
    aspnetcore-runtime-install.exe
    del "aspnetcore-runtime-install.exe"
)

if not exist "%ProgramFiles%\dotnet\shared\Microsoft.WindowsDesktop.App\8.0.5\" (
    curl "https://download.visualstudio.microsoft.com/download/pr/0ff148e7-bbf6-48ed-bdb6-367f4c8ea14f/bd35d787171a1f0de7da6b57cc900ef5/windowsdesktop-runtime-8.0.5-win-x64.exe" -o "windowsdesktop-runtime-install.exe"
    windowsdesktop-runtime-install.exe
    del "windowsdesktop-runtime-install.exe"
)

mkdir "%ProgramFiles%\ThermalSim"

xcopy /S /C /D /H /Y "ThermalSim\" "%ProgramFiles%\ThermalSim\"

timeout /t 1

sc.exe create ThermalSim binpath="%ProgramFiles%\ThermalSim\ThermalSim.Api.exe" start= auto
sc.exe description ThermalSim "The ThermalSim service which connects to MSFS2020 SimConnect and runs the thermal simulation"
sc.exe start ThermalSim