using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ElectricDashboardApi.Infrastructure.Services.Forecast;
using ElectricDashboardApi.Shared.Extensions;

namespace ElectricDashboardApi.Endpoints
{
    [ApiController]
    [Route("api/forecast")]
    public class ForecastEndpoints : ControllerBase
    {
        private readonly IForecastService _forecastService;
        public ForecastEndpoints(IForecastService forecastService){_forecastService=forecastService;}

        [HttpGet("{addressId:guid}")]
        public async Task<IActionResult> Get(Guid addressId)
        {
            var userId = User.GetGuid();
            try
            {
                var response = await _forecastService.GetForecastAsync(addressId, userId);
                return Ok(response);
            }
            catch(UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
