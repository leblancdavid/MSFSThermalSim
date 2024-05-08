namespace ThermalSim.Domain.Thermals
{
    internal class TestingThermalSimulationConfiguration : ThermalSimulationConfiguration
    {
        internal TestingThermalSimulationConfiguration()
        {
            NumberOfThermals = new ValueRangeInt(1, 1);
            SamplingSpeedSeconds = new ValueRangeInt(60, 60);
            DurationMinutes = new ValueRangeInt(420, 420);
            AltitudeFromGround = new ValueRangeDouble(0.0, 0.0);
            SpawnDistance = new ValueRangeDouble(0.0, 0.0);
            RelativeSpawnAltitude = new ValueRangeDouble(-10000.0, -10000);
            Radius = new ValueRangeDouble(20000.0, 20000);
            Height = new ValueRangeDouble(100000.0, 100000.0);
            CoreLiftRate = new ValueRangeDouble(20.0, 20.0);
            CoreRadiusPercent = new ValueRangeDouble(0.8, 0.8);
            CoreTurbulencePercent = new ValueRangeDouble(1.0, 1.0);
            SinkRatePercent = new ValueRangeDouble(-1.0, -1.0);
            SinkTurbulencePercent = new ValueRangeDouble(1.0, 1.0);
            SinkTransitionRadiusPercent = new ValueRangeDouble(0.1, 0.1);
            LiftShapeFactor = new ValueRangeDouble(0.5, 0.5);
            FramesBetweenTurbulence = new ValueRangeInt(240, 240);
            TurbulenceDuration = new ValueRangeInt(120, 120);
            TurbulenceStrengthPercent = new ValueRangeDouble(1.0, 1.0);
            AllowSpawningOnAircraft = true;
        }
    }
}
