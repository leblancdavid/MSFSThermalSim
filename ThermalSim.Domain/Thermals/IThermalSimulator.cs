namespace ThermalSim.Domain.Thermals
{
    public interface IThermalSimulator
    {
        bool IsRunning { get; }
        bool Start();
        void Stop();

        bool InsertThermal();
    }
}
