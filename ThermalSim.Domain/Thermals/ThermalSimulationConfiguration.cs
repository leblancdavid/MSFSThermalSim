namespace ThermalSim.Domain.Thermals
{
    public class ThermalSimulationConfiguration
    {
        public int MinNumberOfThermals { get; set; } = 1;
        public int MaxNumberOfThermals { get; set; } = 1;
        public TimeSpan SamplingSpeed { get; set; } = new TimeSpan(0, 0, 30);
    }
}
