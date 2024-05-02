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
                Latitude = position.Latitude,
                Longitude = position.Longitude,
                Altitude = position.Altitude - 500.0,
                MinAltitudeFromGround = 100.0,
                TotalRadius = 1000.0,
                Height = 2000.0,
                CoreLiftRate = 50.0,
                CoreTurbulencePercent = 0.5,
                SinkRate = -30.0,
                SinkTurbulencePercent = 0.5,
                CoreRadiusPercent = 0.8,
                SinkTransitionRadiusPercent = 0.1,
            };
        }

        public IThermalModel GenerateThermalAtAircraft(AircraftPositionState position)
        {
            lastId++;

            return new CylindricalThermal
            {
                ObjectId = lastId,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now + new TimeSpan(0, 20, 0),
                Latitude = position.Latitude,
                Longitude = position.Longitude,
                Altitude = position.Altitude - 500.0,
                MinAltitudeFromGround = 100.0,
                TotalRadius = 10000.0,
                Height = 20000.0,
                CoreLiftRate = 50.0,
                CoreTurbulencePercent = 0.5,
                SinkRate = -30.0,
                SinkTurbulencePercent = 0.5,
                CoreRadiusPercent = 0.9,
                SinkTransitionRadiusPercent = 0.05,
            };
        }
    }
}
