namespace ThermalSim.Domain.Position
{
    public class AircraftPositionUpdatedEventArgs : EventArgs
    {
        public AircraftPositionUpdatedEventArgs(AircraftPositionState position)
        {
            Position = position;
        }

        public AircraftPositionState Position { get; }
    }
}
