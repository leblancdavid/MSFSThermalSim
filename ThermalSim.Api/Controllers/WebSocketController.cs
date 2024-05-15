using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThermalSim.Api.Services;

namespace ThermalSim.Api.Controllers
{
    public class WebSocketController : ControllerBase
    {
        private readonly IWebSocketService webSocketService;

        public WebSocketController(IWebSocketService webSocketService)
        {
            this.webSocketService = webSocketService;
        }

        [Route("/ws")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                webSocketService.AddWebSocket(await HttpContext.WebSockets.AcceptWebSocketAsync());
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
