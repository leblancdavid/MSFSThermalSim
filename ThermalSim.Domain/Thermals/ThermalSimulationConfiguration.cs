using ThermalSim.Domain.Position;
using ThermalSim.Domain.Turbulence;

namespace ThermalSim.Domain.Thermals
{
    public class ThermalSimulationConfiguration
    {
        public ValueRangeInt NumberOfThermals { get; set; } = new ValueRangeInt(5, 20);
        public ValueRangeInt SamplingSpeedSeconds { get; set; } = new ValueRangeInt(30, 60);
        public ValueRangeInt DurationMinutes { get; set; } = new ValueRangeInt(5, 30);
        public ValueRangeDouble AltitudeFromGround { get; set; } = new ValueRangeDouble(100.0, 1000.0);
        public ValueRangeDouble SpawnDistance { get; set; } = new ValueRangeDouble(0.01, 0.05); //This is in gps degrees
        public ValueRangeDouble RelativeSpawnAltitude { get; set; } = new ValueRangeDouble(-1000.0, 0.0);
        public ValueRangeDouble Radius { get; set; } = new ValueRangeDouble(5000.0, 10000.0);
        public ValueRangeDouble Height { get; set; } = new ValueRangeDouble(1000.0, 5000.0);
        public ValueRangeDouble CoreLiftRate { get; set; } = new ValueRangeDouble(5.0, 30.0);
        public ValueRangeDouble CoreRadiusPercent { get; set; } = new ValueRangeDouble(0.85, 0.90);
        public ValueRangeDouble CoreTurbulencePercent { get; set; } = new ValueRangeDouble(0.0, 2.0);
        public ValueRangeDouble SinkRate { get; set; } = new ValueRangeDouble(-20.0, - 5.0);
        public ValueRangeDouble SinkTurbulencePercent { get; set; } = new ValueRangeDouble(0.0, 2.0);
        public ValueRangeDouble SinkTransitionRadiusPercent { get; set; } = new ValueRangeDouble(0.0, 0.1);
        public ValueRangeDouble LiftShapeFactor { get; set; } = new ValueRangeDouble(0.0, 1.0);
        public ValueRangeDouble WindSpeed { get; set; } = new ValueRangeDouble(0.0, 50.0);
        public ValueRangeDouble WindDirection { get; set; } = new ValueRangeDouble(0.0, 360.0);
        public double ReplaceDistance { get; set; } = 50000;
        public ValueRangeInt FramesBetweenTurbulence { get; set; } = new ValueRangeInt(30, 240);
        public ValueRangeInt TurbulenceDuration { get; set; } = new ValueRangeInt(30, 120);
        public ValueRangeDouble TurbulenceStrength { get; set; } = new ValueRangeDouble(20.0, 80.0);

        public ThermalProperties GenerateRandomThermalProperties(Random random, AircraftPositionState position)
        {
            var coreRadius = CoreRadiusPercent.GetRandomValue(random);
            var transitionRadius = SinkTransitionRadiusPercent.GetRandomValue(random);
            var coreRate = CoreLiftRate.GetRandomValue(random);

            var properties = new ThermalProperties()
            {
                StartTime = DateTime.Now,
                EndTime = DateTime.Now + new TimeSpan(0, DurationMinutes.GetRandomValue(random), 0),
                Latitude = position.Latitude + RandomInvert(random) * SpawnDistance.GetRandomValue(random),
                Longitude = position.Longitude + RandomInvert(random) * SpawnDistance.GetRandomValue(random),
                Altitude = position.Altitude + RelativeSpawnAltitude.GetRandomValue(random),
                MinAltitudeFromGround = AltitudeFromGround.Min,
                MaxAltitudeFromGround = AltitudeFromGround.Max,
                Height = Height.GetRandomValue(random),
                TotalRadius = Radius.GetRandomValue(random),
                CoreLiftRate = coreRate,
                CoreTurbulencePercent = CoreTurbulencePercent.GetRandomValue(random),
                CoreRadiusPercent = coreRadius,
                SinkRate = coreRate * -0.80,
                SinkTurbulencePercent = SinkTurbulencePercent.GetRandomValue(random),
                SinkTransitionRadiusPercent = coreRadius + transitionRadius,
                WindSpeed = WindSpeed.GetRandomValue(random),
                WindDirection = WindDirection.GetRandomValue(random),
                LiftShapeFactor = LiftShapeFactor.GetRandomValue(random)
            };

            return properties;
        }

        public TurbulenceProperties GenerateRandomTurbulenceProperties(Random random)
        {
            var strength = TurbulenceStrength.GetRandomValue(random);
            return new TurbulenceProperties()
            {
                FramesBetweenTurbulence = FramesBetweenTurbulence,
                TurbulenceDuration = TurbulenceDuration,
                TurbulenceStrength = new ValueRangeDouble(strength / 10.0, strength)
            };
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
