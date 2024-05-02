using System.Windows.Forms;
using ThermalSim.Domain.Position;

namespace ThermalSim.Domain.Thermals
{
    public class ThermalSimulationConfiguration
    {
        public ValueRangeInt NumberOfThermals { get; set; } = new ValueRangeInt(5, 30);
        public ValueRangeInt SamplingSpeedSeconds { get; set; } = new ValueRangeInt(30, 60);
        public ValueRangeInt DurationMinutes { get; set; } = new ValueRangeInt(5, 30);
        public ValueRangeDouble AltitudeFromGround { get; set; } = new ValueRangeDouble(50.0, 200.0);
        public ValueRangeDouble SpawnDistance { get; set; } = new ValueRangeDouble(0.01, 0.02); //This is in gps degrees
        public ValueRangeDouble RelativeSpawnAltitude { get; set; } = new ValueRangeDouble(-1000.0, 0.0);
        public ValueRangeDouble Radius { get; set; } = new ValueRangeDouble(200.0, 800.0);
        public ValueRangeDouble Height { get; set; } = new ValueRangeDouble(1000.0, 5000.0);
        public ValueRangeDouble CoreLiftRate { get; set; } = new ValueRangeDouble(5.0, 50.0);
        public ValueRangeDouble CoreRadiusPercent { get; set; } = new ValueRangeDouble(0.7, 0.9);
        public ValueRangeDouble CoreTurbulencePercent { get; set; } = new ValueRangeDouble(0.0, 2.0);
        public ValueRangeDouble SinkRate { get; set; } = new ValueRangeDouble(-50.0, - 5.0);
        public ValueRangeDouble SinkTurbulencePercent { get; set; } = new ValueRangeDouble(0.0, 2.0);
        public ValueRangeDouble SinkTransitionRadiusPercent { get; set; } = new ValueRangeDouble(0.0, 0.1);
        public ValueRangeDouble WindSpeed { get; set; } = new ValueRangeDouble(0.0, 50.0);
        public ValueRangeDouble WindDirection { get; set; } = new ValueRangeDouble(0.0, 360.0);

        public ThermalProperties GenerateRandomThermalProperties(Random random, AircraftPositionState position)
        {
            var coreRadius = CoreRadiusPercent.GetRandomValue(random);
            var transitionRadius = SinkTransitionRadiusPercent.GetRandomValue(random);

            var properties = new ThermalProperties()
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now + new TimeSpan(0, DurationMinutes.GetRandomValue(random), 0),
                Latitude = position.Latitude + RandomInvert(random) * SpawnDistance.GetRandomValue(random),
                Longitude = position.Longitude + RandomInvert(random) * SpawnDistance.GetRandomValue(random),
                Altitude = position.Altitude + RelativeSpawnAltitude.GetRandomValue(random),
                MinAltitudeFromGround = AltitudeFromGround.GetRandomValue(random),
                Height = Height.GetRandomValue(random),
                TotalRadius = Radius.GetRandomValue(random),
                CoreLiftRate = CoreLiftRate.GetRandomValue(random),
                CoreTurbulencePercent = CoreTurbulencePercent.GetRandomValue(random),
                CoreRadiusPercent = coreRadius,
                SinkRate = SinkRate.GetRandomValue(random),
                SinkTurbulencePercent = SinkTurbulencePercent.GetRandomValue(random),
                SinkTransitionRadiusPercent = coreRadius + transitionRadius,
                WindSpeed = WindSpeed.GetRandomValue(random),
                WindDirection = WindDirection.GetRandomValue(random),
            };

            return properties;
        }

        public DateTime GetNextSampleTime(Random random)
        {
            return DateTime.Now + new TimeSpan(0, 0, SamplingSpeedSeconds.GetRandomValue(random));
        }

        private double RandomInvert(Random random)
        {
            return random.NextDouble() > 0.5 ? 1.0 : -1.0;
        }



    }
}
