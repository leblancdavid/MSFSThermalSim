using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using ThermalSim.Domain.Notifications;

namespace ThermalSim.Api.Services
{
    public class WebSocketService : IEventNotifier<WebSocket>
    {
        private readonly ILogger<WebSocketService> logger;
        private List<WebSocket> _webSockets = new List<WebSocket>();
        public WebSocketService(ILogger<WebSocketService> logger)
        {
            this.logger = logger;
        }


        public async Task Accept(WebSocket webSocket)
        {
           
            _webSockets.Add(webSocket);

            var buffer = Encoding.UTF8.GetBytes("Connection Accepted!");
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), 
                WebSocketMessageType.Text, 
                true, 
                CancellationToken.None);
        }

        public async Task NotifyAsync<T>(T message)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(message);

                var buffer = Encoding.UTF8.GetBytes(jsonString);

                var openSockets = _webSockets.Where(x => x.State == WebSocketState.Open);
                foreach (var socket in openSockets)
                {
                    await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }

                _webSockets.RemoveAll(x => x.State != WebSocketState.Open && x.State != WebSocketState.Connecting);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }
    }
}
