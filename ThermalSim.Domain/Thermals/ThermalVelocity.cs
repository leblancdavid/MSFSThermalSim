using Microsoft.FlightSimulator.SimConnect;
using System.Runtime.InteropServices;
using ThermalSim.Domain.Connection;

namespace ThermalSim.Domain.Thermals
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ThermalVelocity
    {
        [SimConnectVariable(Name = "ACCELERATION BODY X", Unit = "Feet per second squared", Type = SIMCONNECT_DATATYPE.FLOAT64, SetType = SetType.None)]
        public double AccelerationBodyX;
        [SimConnectVariable(Name = "ACCELERATION BODY Y", Unit = "Feet per second squared", Type = SIMCONNECT_DATATYPE.FLOAT64, SetType = SetType.None)]
        public double AccelerationBodyY;
        [SimConnectVariable(Name = "ACCELERATION BODY Z", Unit = "Feet per second squared", Type = SIMCONNECT_DATATYPE.FLOAT64, SetType = SetType.None)]
        public double AccelerationBodyZ;
    }
}
