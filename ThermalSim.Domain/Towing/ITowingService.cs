namespace ThermalSim.Domain.Towing
{
    public interface ITowingService
    {
        bool IsTowing { get; }
        double TowingSpeed { get; set; }
        bool StartTowing();
        bool StopTowing();
    }
}
