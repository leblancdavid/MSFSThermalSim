using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace ThermalSim.Api.Services
{
    public class WebSocketService : IWebSocketService
    {
        private List<WebSocket> _webSockets = new List<WebSocket>();
        public void AddWebSocket(WebSocket webSocket)
        {
            _webSockets.Add(webSocket);
        }

        public async Task NotifyAsync<T>(T message)
        {
            string jsonString = JsonSerializer.Serialize(message);

            var buffer = Encoding.UTF8.GetBytes(jsonString);
            
            var openSockets = _webSockets.Where(x => x.State != WebSocketState.Open);
            foreach(var socket in openSockets)
            {
                await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }

            _webSockets.RemoveAll(x =>  x.State != WebSocketState.Open || x.State != WebSocketState.Connecting);
        }
    }
}
