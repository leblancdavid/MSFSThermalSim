using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThermalSim.Api.Services;

namespace ThermalSim.Api.Controllers
{
    [Route("/api/ws")]
    [ApiController]
    public class WebSocketController : ControllerBase
    {
        private readonly IWebSocketService webSocketService;

        public WebSocketController(IWebSocketService webSocketService)
        {
            this.webSocketService = webSocketService;
        }

        [HttpGet]
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
