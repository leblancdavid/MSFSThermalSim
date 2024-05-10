using ThermalSim.Domain.Thermals;

namespace ThermalSim.Domain.Turbulence
{
    public class TurbulenceProperties
    {
        public ValueRangeInt FramesBetweenTurbulence { get; set; } = new ValueRangeInt(120, 1200);
        public ValueRangeInt TurbulenceDuration { get; set; } = new ValueRangeInt(60, 360);
        public ValueRangeDouble TurbulenceStrength { get; set; } = new ValueRangeDouble(10.0, 100.0);

        public double x_Scaler { get; set; } = 0.75;
        public double y_Scaler { get; set; } = 0.25;
        public double z_Scaler { get; set; } = 1.5;
    }
}
