using ThermalSim.Domain.Position;
using ThermalSim.Domain.Turbulence;

namespace ThermalSim.Domain.Thermals
{
    public class FixedPositionThermalGenerator : IThermalGenerator
    {
        private uint lastId = 0;
        private Random random = new Random();
        private ThermalSimulationConfiguration configuration = new ThermalSimulationConfiguration();
        public FixedPositionThermalGenerator()
        {
            
        }

        public ThermalSimulationConfiguration Configuration => configuration;

        public IThermalModel GenerateThermalAroundAircraft(AircraftPositionState position)
        {
            lastId++;

            return new CylindricalThermal
            {
                ObjectId = lastId,
                Properties = configuration.GenerateRandomThermalProperties(random, position),
                TurbulenceModel = new CounterBasedTurbulenceModel(configuration.GenerateRandomTurbulenceProperties(random))
            };
        }

        public IThermalModel GenerateThermalAtAircraft(AircraftPositionState position)
        {
            lastId++;

            var thermal = new CylindricalThermal
            {
                ObjectId = lastId,
                Properties = configuration.GenerateRandomThermalProperties(random, position),
                TurbulenceModel = new CounterBasedTurbulenceModel(configuration.GenerateRandomTurbulenceProperties(random))
            };

            thermal.Properties.Longitude = position.Longitude;
            thermal.Properties.Latitude = position.Latitude;
            thermal.Properties.Altitude = position.Altitude;

            return thermal;
        }

        public DateTime GetNextSampleTime()
        {
            return Configuration.GetNextSampleTime(random);
        }
    }
}
