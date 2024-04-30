using Microsoft.FlightSimulator.SimConnect;

namespace ThermalSim.Domain
{
    public interface ISimConnection
    {
        bool IsConnected { get; }
        SimConnect? Connection { get; }
        void Connect();
        void Disconnect();
    }
}
