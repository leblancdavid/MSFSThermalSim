using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThermalSim.Domain.Connection;

namespace ThermalSim.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SimConnectionController : ControllerBase
    {
        private readonly ISimConnection simConnection;

        public SimConnectionController(ISimConnection simConnection)
        {
            this.simConnection = simConnection;
        }

        [HttpPut]
        public IActionResult Connect()
        {
            try
            {
                var result = simConnection.Connect();
                if(result)
                    return Ok();

                return BadRequest("Unable to connect to simulator");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
