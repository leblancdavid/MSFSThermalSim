using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThermalSim.Domain.Connection;
using ThermalSim.Domain.Thermals;

namespace ThermalSim.Api.Controllers
{
    [Route("api/thermals")]
    [ApiController]
    public class ThermalsController : ControllerBase
    {
        private readonly IThermalSimulator thermalSimulator;

        public ThermalsController(IThermalSimulator thermalSimulator)
        {
            this.thermalSimulator = thermalSimulator;
        }

        [HttpPut]
        public IActionResult Start()
        {
            try
            {
                var result = thermalSimulator.Start();
                if (result)
                    return Ok();

                return BadRequest("Unable to start the thermal simulation");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("running")]
        public bool IsConnected()
        {
            return thermalSimulator.IsRunning;
        }

        [HttpDelete]
        public IActionResult Stop()
        {
            try
            {
                thermalSimulator.Stop();
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("insert-thermal")]
        public IActionResult AddThermal()
        {
            try
            {
                thermalSimulator.InsertThermal();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
