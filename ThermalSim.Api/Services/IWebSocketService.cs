using System.Net.WebSockets;
using ThermalSim.Domain.Notifications;

namespace ThermalSim.Api.Services
{
    public interface IWebSocketService : IEventNotifier
    {
        void AddWebSocket(WebSocket webSocket);
    }
}
