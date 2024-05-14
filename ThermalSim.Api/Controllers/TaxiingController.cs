using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThermalSim.Domain.Thermals;
using ThermalSim.Domain.Towing;

namespace ThermalSim.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxiingController : ControllerBase
    {
        private readonly ITaxiingService taxiingService;

        public TaxiingController(ITaxiingService taxiingService)
        {
            this.taxiingService = taxiingService;
        }

        [HttpPut]
        public IActionResult Start()
        {
            try
            {
                var result = taxiingService.StartTaxiing();
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
            return taxiingService.IsTaxiing;
        }

        [HttpDelete]
        public IActionResult Stop()
        {
            try
            {
                taxiingService.StopTaxiing();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("speed/{speed}")]
        public IActionResult SetTaxiingSpeed(double speed)
        {
            try
            {
                taxiingService.TaxiingSpeed = speed;
                return Ok(taxiingService.TaxiingSpeed);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("speed")]
        public double GetTaxiingSpeed()
        {
            return taxiingService.TaxiingSpeed;
        }
    }
}
