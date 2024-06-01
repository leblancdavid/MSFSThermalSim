using ThermalSim.Domain.Position;
using ThermalSim.Domain.Turbulence;

namespace ThermalSim.Domain.Thermals
{
    public class ThermalSimulationConfiguration
    {
        public ValueRangeInt NumberOfThermals { get; set; } = new ValueRangeInt(5, 10);
        public ValueRangeInt SamplingSpeedSeconds { get; set; } = new ValueRangeInt(30, 60);
        public ValueRangeInt DurationMinutes { get; set; } = new ValueRangeInt(5, 30);
        public ValueRangeDouble AltitudeFromGround { get; set; } = new ValueRangeDouble(100.0, 500.0);
        public ValueRangeDouble SpawnDistance { get; set; } = new ValueRangeDouble(0.01, 0.03); //This is in gps degrees
        public ValueRangeDouble RelativeSpawnAltitude { get; set; } = new ValueRangeDouble(-1000.0, 0.0);
        public ValueRangeDouble Radius { get; set; } = new ValueRangeDouble(1000.0, 2000.0);
        public ValueRangeDouble Height { get; set; } = new ValueRangeDouble(3000.0, 10000.0);
        public ValueRangeDouble CoreLiftRate { get; set; } = new ValueRangeDouble(10.0, 20.0);
        public ValueRangeDouble CoreRadiusPercent { get; set; } = new ValueRangeDouble(0.80, 0.85);
        public ValueRangeDouble CoreTurbulencePercent { get; set; } = new ValueRangeDouble(0.0, 2.0);
        public ValueRangeDouble SinkRatePercent { get; set; } = new ValueRangeDouble(-1.5, -0.5);
        public ValueRangeDouble SinkTurbulencePercent { get; set; } = new ValueRangeDouble(0.0, 2.0);
        public ValueRangeDouble SinkTransitionRadiusPercent { get; set; } = new ValueRangeDouble(0.05, 0.1);
        public ValueRangeDouble LiftShapeFactor { get; set; } = new ValueRangeDouble(0.0, 1.0);
        public ValueRangeDouble WindSpeed { get; set; } = new ValueRangeDouble(0.0, 50.0);
        public ValueRangeDouble WindDirection { get; set; } = new ValueRangeDouble(0.0, 360.0);
        public double ReplaceDistance { get; set; } = 25000;
        public ValueRangeInt FramesBetweenTurbulence { get; set; } = new ValueRangeInt(60, 120);
        public ValueRangeInt TurbulenceDuration { get; set; } = new ValueRangeInt(120, 360);
        public ValueRangeDouble TurbulenceStrengthPercent { get; set; } = new ValueRangeDouble(1.0, 1.5);
        public bool AllowSpawningOnAircraft { get; set; } = false;
        public double PitchTurbulenceModifier { get; set; } = 0.0;
        public double YawTurbulenceModifier { get; set; } = 0.0;
        public double RollTurbulenceModifier { get; set; } = 0.0;

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
                SinkRate = coreRate * SinkRatePercent.GetRandomValue(random),
                SinkTurbulencePercent = SinkTurbulencePercent.GetRandomValue(random),
                SinkTransitionRadiusPercent = coreRadius + transitionRadius,
                WindSpeed = WindSpeed.GetRandomValue(random),
                WindDirection = WindDirection.GetRandomValue(random),
                LiftShapeFactor = LiftShapeFactor.GetRandomValue(random)
            };

            return properties;
        }

        public TurbulenceProperties GenerateRandomTurbulenceProperties(Random random, double coreLiftRate)
        {
            var strength = coreLiftRate * TurbulenceStrengthPercent.GetRandomValue(random);
            return new TurbulenceProperties()
            {
                FramesBetweenTurbulence = FramesBetweenTurbulence,
                TurbulenceDuration = TurbulenceDuration,
                TurbulenceStrength = new ValueRangeDouble(strength / 10.0, strength),
                PitchTurbulenceModifier = PitchTurbulenceModifier,
                YawTurbulenceModifier = YawTurbulenceModifier,
                RollTurbulenceModifier = RollTurbulenceModifier
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
