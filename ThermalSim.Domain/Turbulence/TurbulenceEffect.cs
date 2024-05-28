using Microsoft.FlightSimulator.SimConnect;
using System.Runtime.InteropServices;
using ThermalSim.Domain.Connection;

namespace ThermalSim.Domain.Turbulence
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TurbulenceEffect
    {
        [SimConnectVariable(Name = "ROTATION ACCELERATION BODY X", Unit = "Feet per second squared", Type = SIMCONNECT_DATATYPE.FLOAT64, SetType = SetType.None)]
        public double RotationAccelerationBodyX;
        [SimConnectVariable(Name = "ROTATION ACCELERATION BODY Y", Unit = "Feet per second squared", Type = SIMCONNECT_DATATYPE.FLOAT64, SetType = SetType.None)]
        public double RotationAccelerationBodyY;
        [SimConnectVariable(Name = "ROTATION ACCELERATION BODY Z", Unit = "Feet per second squared", Type = SIMCONNECT_DATATYPE.FLOAT64, SetType = SetType.None)]
        public double RotationAccelerationBodyZ;
    }
}
