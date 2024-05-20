using Microsoft.FlightSimulator.SimConnect;
using System.Runtime.InteropServices;
using ThermalSim.Domain.Connection;

namespace ThermalSim.Domain.Thermals
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ThermalSimEnabled
    {
        //We are using the FOLDING WING RIGHT PERCENT variable as a flag to indicate the thermalsim is in use
        [SimConnectVariable(Name = "FOLDING WING RIGHT PERCENT", Unit = "Percent", Type = SIMCONNECT_DATATYPE.FLOAT64)]
        public double ThermalSimIsEnabled;
    }
}
