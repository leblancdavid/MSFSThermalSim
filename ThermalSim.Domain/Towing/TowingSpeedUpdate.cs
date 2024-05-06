using Microsoft.FlightSimulator.SimConnect;
using System.Runtime.InteropServices;
using ThermalSim.Domain.Connection;

namespace ThermalSim.Domain.Towing
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TowingSpeedUpdate
    {
        [SimConnectVariable(Name = "VELOCITY BODY X", Unit = "Feet per second", Type = SIMCONNECT_DATATYPE.FLOAT64)]
        public double VelocityBodyZ;
        [SimConnectVariable(Name = "ROTATION ACCELERATION BODY Y", Unit = "Feet per second squared", Type = SIMCONNECT_DATATYPE.FLOAT64, SetType = SetType.None)]
        public double RotationAccelerationBodyY;
        [SimConnectVariable(Name = "ROTATION ACCELERATION BODY Z", Unit = "Feet per second squared", Type = SIMCONNECT_DATATYPE.FLOAT64, SetType = SetType.None)]
        public double RotationAccelerationBodyZ;
    }
}
