using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Thermals
{
    public interface IThermalGenerator
    {
        IThermalModel GenerateThermalAroundAircraft(AircraftPositionState position);
        IThermalModel GenerateThermalAtAircraft(AircraftPositionState position);

    }
}
