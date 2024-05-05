namespace ThermalSim.Domain.Towing
{
    public interface ITowingService
    {
        bool IsTowing { get; }
        bool StartTowing();
        bool StopTowing();
    }
}
