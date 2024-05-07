namespace ThermalSim.Domain.Thermals
{
    public class ThermalProperties
    {
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; } = DateTime.Now + new TimeSpan(1, 0, 0);
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
        public double Height { get; set; }
        public double MinAltitudeFromGround { get; set; }
        public double MaxAltitudeFromGround { get; set; }
        public double TopAltitude => Altitude + Height;
        public double TotalRadius { get; set; }
        public double CoreLiftRate { get; set; }
        public double CoreTurbulencePercent { get; set; }
        public double SinkRate { get; set; }
        public double SinkTurbulencePercent { get; set; }
        public double CoreRadiusPercent { get; set; }
        public double SinkTransitionRadiusPercent { get; set; }
        public double WindSpeed { get; set; }
        public double WindDirection { get; set; }
        public double LiftShapeFactor { get; set; } = 1.0;
    }
}
