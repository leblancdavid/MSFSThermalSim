using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThermalSim.Domain.Thermals;
using ThermalSim.Domain.Towing;

namespace ThermalSim.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TowingController : ControllerBase
    {
        private readonly ITowingService towingService;

        public TowingController(ITowingService towingService)
        {
            this.towingService = towingService;
        }

        [HttpPut]
        public IActionResult Start()
        {
            try
            {
                var result = towingService.StartTowing();
                if (result)
                    return Ok();

                return BadRequest("Unable to start towing");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("running")]
        public bool IsConnected()
        {
            return towingService.IsTowing;
        }

        [HttpDelete]
        public IActionResult Stop()
        {
            try
            {
                towingService.StopTowing();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("speed/{speed}")]
        public IActionResult SetTowingSpeed(double speed)
        {
            try
            {
                towingService.TowingSpeed = speed;
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
