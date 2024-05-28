using Microsoft.FlightSimulator.SimConnect;
using System.Runtime.InteropServices;
using ThermalSim.Domain.Connection;

namespace ThermalSim.Domain.Turbulence
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TurbulenceEffect
    {
        [SimConnectVariable(Name = "ROTATION VELOCITY BODY X", Unit = "Feet per second", Type = SIMCONNECT_DATATYPE.FLOAT64, SetType = SetType.None)]
        public double RotationVelocityBodyX;
        [SimConnectVariable(Name = "ROTATION VELOCITY BODY Y", Unit = "Feet per second", Type = SIMCONNECT_DATATYPE.FLOAT64, SetType = SetType.None)]
        public double RotationVelocityBodyY;
        [SimConnectVariable(Name = "ROTATION VELOCITY BODY Z", Unit = "Feet per second", Type = SIMCONNECT_DATATYPE.FLOAT64, SetType = SetType.None)]
        public double RotationVelocityBodyZ;
    }
}
