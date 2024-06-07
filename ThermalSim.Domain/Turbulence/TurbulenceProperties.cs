using ThermalSim.Domain.Thermals;

namespace ThermalSim.Domain.Turbulence
{
    public class TurbulenceProperties
    {
        public ValueRangeInt FramesBetweenTurbulence { get; set; } = new ValueRangeInt(120, 1200);
        public ValueRangeInt TurbulenceDuration { get; set; } = new ValueRangeInt(60, 360);
        public ValueRangeDouble TurbulenceStrength { get; set; } = new ValueRangeDouble(50.0, 100.0);

        public double PitchTurbulenceModifier { get; set; } = 0.0;
        public double YawTurbulenceModifier { get; set; } = 0.0;
        public double RollTurbulenceModifier { get; set; } = 0.0;
    }
}
