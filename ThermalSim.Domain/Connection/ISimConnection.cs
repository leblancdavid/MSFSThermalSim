using Microsoft.FlightSimulator.SimConnect;
using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Connection
{
    public interface ISimConnection
    {
        bool IsConnected { get; }
        SimConnect? Connection { get; }
        event EventHandler<AircraftPositionUpdatedEventArgs>? AircraftPositionUpdated;
        void Connect();
        void Disconnect();
    }
}
