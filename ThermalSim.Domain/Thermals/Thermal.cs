namespace ThermalSim.Domain.Thermals
{
    public class Thermal
    {
        public uint ObjectId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Altitude { get; set; }
        public float Radius { get; set; }
        public float Height { get; set; }
        public float CoreRate { get; set; }
        public float CoreTurbulence { get; set; }
        public float SinkRate { get; set; }
        public float SinkTurbulence { get; set; }
        public float CoreSize { get; set; }
        public float CoreTransitionSize { get; set; }
        public float SinkLayerSize { get; set; }
        public float SinkTransitionSize { get; set; }
    }
}
