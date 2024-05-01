using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Thermals
{
    public class FixedPositionThermalGenerator : IThermalGenerator
    {
        private uint lastId = 0;

        public FixedPositionThermalGenerator()
        {
            
        }

        public IThermalModel GenerateThermalAroundAircraft(AircraftPositionState position)
        {
            lastId++;

            return new CylindricalThermal
            {
                ObjectId = lastId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now + new TimeSpan(0, 20, 0),
                Latitude = (float)position.Latitude,
                Longitude = (float)position.Longitude,
                Altitude = (float)position.Altitude,
                Radius = 1000.0f,
                Height = 1000.0f,
                CoreRate = 1000.0f,
                CoreTurbulence = 1.0f,
                SinkRate = -200.0f,
                SinkTurbulence = 0.0f,
                CoreSize = 500.0f,
                CoreTransitionSize = 100.0f,
                SinkLayerSize = 100.0f,
                SinkTransitionSize = 10.0f,
            };
        }
    }
}
