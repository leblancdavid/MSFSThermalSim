namespace ThermalSim.Domain.Notifications
{
    public interface IEventNotifier<T>
    {
        Task Accept(T connection);
        Task NotifyAsync<TPayload>(TPayload message);
    }
}
