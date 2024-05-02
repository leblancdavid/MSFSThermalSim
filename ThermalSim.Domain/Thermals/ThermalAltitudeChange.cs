using Microsoft.FlightSimulator.SimConnect;
using System.Runtime.InteropServices;
using ThermalSim.Domain.Connection;

namespace ThermalSim.Domain.Thermals
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ThermalAltitudeChange
    {
        [SimConnectVariable(Name = "PLANE ALTITUDE", Unit = "Feet", Type = SIMCONNECT_DATATYPE.FLOAT64)]
        public double Altitude;
    }
}
