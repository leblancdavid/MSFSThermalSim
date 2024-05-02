namespace ThermalSim.Domain.Position
{
    public interface IAircraftStateTracker
    {
        int SamplingLength { get; set; }
        AircraftPositionState? LastState { get; }
        AircraftPositionState? FirstState { get; }
        AircraftStateChangeInfo? AircraftStateChangeInfo { get; }
        void UpdatePosition(AircraftPositionState state);


    }
}
