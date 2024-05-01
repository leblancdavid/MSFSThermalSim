using Microsoft.FlightSimulator.SimConnect;
using System.Runtime.InteropServices;
using ThermalSim.Domain.Connection;

namespace ThermalSim.Domain.Thermals
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ThermalVelocity
    {
        [SimConnectVariable(Name = "VERTICAL SPEED", Unit = "Feet per second", Type = SIMCONNECT_DATATYPE.FLOAT64)]
        public double VerticalSpeed;
        [SimConnectVariable(Name = "VELOCITY BODY X", Unit = "Feet per second", Type = SIMCONNECT_DATATYPE.FLOAT64)]
        public double VelocityBodyY;
        [SimConnectVariable(Name = "VELOCITY BODY Y", Unit = "Feet per second", Type = SIMCONNECT_DATATYPE.FLOAT64)]
        public double VelocityBodyZ;
        [SimConnectVariable(Name = "ROTATION ACCELERATION BODY X", Unit = "Feet per second", Type = SIMCONNECT_DATATYPE.FLOAT64)]
        public double RotationAccelerationBodyX;
    }
}
