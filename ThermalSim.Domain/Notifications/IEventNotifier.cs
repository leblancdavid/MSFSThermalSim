namespace ThermalSim.Domain.Notifications
{
    public interface IEventNotifier
    {
        Task NotifyAsync<T>(T message);
    }
}
