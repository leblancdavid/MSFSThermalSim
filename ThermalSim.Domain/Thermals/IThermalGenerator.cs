using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Thermals
{
    public interface IThermalGenerator
    {
        ThermalSimulationConfiguration Configuration { get; }
        IThermalModel GenerateThermalAroundAircraft(AircraftPositionState position);
        IThermalModel GenerateThermalAtAircraft(AircraftPositionState position);
        DateTime GetNextSampleTime();

    }
}
