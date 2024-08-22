using System;
using System.Security.Claims;
using IceCareNigLtd.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IceCareNigLtd.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        [Route("GetData")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> GetDashboardData([FromQuery] string adminUsername)
        public async Task<IActionResult> GetDashboardData()
        {
            var adminUsername = User?.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(adminUsername))
            {
                return BadRequest("Unable to retrieve the username from the token.");
            }

            var response = await _dashboardService.GetDashboardDataAsync(adminUsername);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Data);
        }

        [HttpPost]
        [Route("UpdateDollarRate")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateDollarRate([FromBody] decimal newDollarRate)
        {
            if (newDollarRate <= 0)
            {
                return BadRequest("Invalid dollar rate.");
            }

            var result = await _dashboardService.UpdateDollarRateAsync(newDollarRate);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("UpdatePhoneNumbers")]
        public async Task<IActionResult> UpdatePhoneNumbers([FromBody] List<string> phoneNumbers)
        {
            var response = await _dashboardService.UpdateCompanyPhoneNumbersAsync(phoneNumbers);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }
            return Ok(response.Message);
        }
    }
}

