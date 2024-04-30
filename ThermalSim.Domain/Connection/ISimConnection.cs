using Microsoft.FlightSimulator.SimConnect;

namespace ThermalSim.Domain.Connection
{
    public interface ISimConnection
    {
        bool IsConnected { get; }
        SimConnect? Connection { get; }
        void Connect();
        void Disconnect();
    }
}
