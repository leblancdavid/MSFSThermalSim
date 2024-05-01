using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Thermals
{
    public class FixedPositionThermalGenerator : IThermalGenerator
    {
        public FixedPositionThermalGenerator()
        {
            
        }

        public Thermal GenerateThermalAroundAircraft(AircraftPositionState position)
        {
            throw new NotImplementedException();
        }
    }
}
