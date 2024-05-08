namespace ThermalSim.Domain.Thermals
{
    public interface IThermalSimulator
    {
        bool IsRunning { get; }
        ThermalSimulationConfiguration Configuration { get; set; }
        bool Start();
        void Stop();

        bool InsertThermal();
    }
}
