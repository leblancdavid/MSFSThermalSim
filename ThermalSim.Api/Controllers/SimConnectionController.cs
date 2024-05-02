using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThermalSim.Domain.Connection;

namespace ThermalSim.Api.Controllers
{
    [Route("api/sim-connection")]
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

        [HttpGet("/connected")]
        public bool IsConnected()
        {
            return simConnection.IsConnected;
        }

        [HttpDelete]
        public IActionResult Disconnect()
        {
            try
            {
                simConnection.Disconnect();
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("/add-thermal")]
        public IActionResult AddNewThermal()
        {
            try
            {
                simConnection.Disconnect();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
