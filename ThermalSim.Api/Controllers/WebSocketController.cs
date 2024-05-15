using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using ThermalSim.Api.Services;
using ThermalSim.Domain.Notifications;

namespace ThermalSim.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WebSocketController : ControllerBase
    {
        private readonly IEventNotifier<WebSocket> webSocketService;
        private readonly ILogger<WebSocketController> logger;

        public WebSocketController(IEventNotifier<WebSocket> webSocketService, ILogger<WebSocketController> logger)
        {
            this.webSocketService = webSocketService;
            this.logger = logger;
        }

        [HttpGet("/ws")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                var socketFinishedTcs = new TaskCompletionSource<object>();
                await webSocketService.Accept(socket);

                await socketFinishedTcs.Task;
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
