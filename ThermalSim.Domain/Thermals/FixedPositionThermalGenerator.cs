using ThermalSim.Domain.Position;
using ThermalSim.Domain.Turbulence;

namespace ThermalSim.Domain.Thermals
{
    public class FixedPositionThermalGenerator : IThermalGenerator
    {
        private uint lastId = 0;
        private Random random = new Random();
        public FixedPositionThermalGenerator()
        {
            
        }

        public ThermalSimulationConfiguration Configuration { get; set; } = new ThermalSimulationConfiguration();

        public IThermalModel GenerateThermalAroundAircraft(AircraftPositionState position)
        {
            lastId++;

            var thermalProp = Configuration.GenerateRandomThermalProperties(random, position);
            return new CylindricalThermal
            {
                ObjectId = lastId,
                Properties = thermalProp,
                TurbulenceModel = new DirectionalTurbulenceModel(Configuration.GenerateRandomTurbulenceProperties(random, thermalProp.CoreLiftRate))
            };
        }

        public IThermalModel GenerateThermalAtAircraft(AircraftPositionState position)
        {
            lastId++;

            var thermalProp = Configuration.GenerateRandomThermalProperties(random, position);
            var thermal = new CylindricalThermal
            {
                ObjectId = lastId,
                Properties = thermalProp,
                TurbulenceModel = new DirectionalTurbulenceModel(Configuration.GenerateRandomTurbulenceProperties(random, thermalProp.CoreLiftRate))
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
