namespace ThermalSim.Domain.Thermals
{
    public class ThermalSimulationConfiguration
    {
        public int MinNumberOfThermals { get; set; } = 5;
        public int MaxNumberOfThermals { get; set; } = 20;
        public TimeSpan SamplingSpeed { get; set; } = new TimeSpan(0, 0, 30);
    }
}
