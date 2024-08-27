using System;
using System.Security.Claims;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Network;
using IceCareNigLtd.Api.Models.Users;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Core.Interfaces;
using IceCareNigLtd.Core.Services;
using IceCareNigLtd.Core.Utils;
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

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("AddCompanyAccount")]
        public async Task<IActionResult> AddCompanyAccount([FromBody] CompanyAccountsDto companyAccountsDto)
        {
            if (companyAccountsDto == null)
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = "Accounts data cannot be null.",
                    Errors = new List<string> { "Invalid input." }
                });
            }

            var requiredFields = new Dictionary<string, string>
            {
                { nameof(companyAccountsDto.AccountName), companyAccountsDto.AccountName },
                { nameof(companyAccountsDto.AccountNumber), companyAccountsDto.AccountNumber },
                { nameof(companyAccountsDto.BankName), companyAccountsDto.BankName }
            };
            foreach (var field in requiredFields)
            {
                if (string.IsNullOrEmpty(field.Value))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Success = false,
                        Message = $"{field.Key} cannot be empty.",
                        Errors = new List<string> { "Invalid input." }
                    });
                }
            }

            var response = await _dashboardService.AddCompanyAccountAsyn(companyAccountsDto);

            if (!response.Success)
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = response.Message,
                    Errors = new List<string> { "Failed to add bank." }
                });
            }

            return CreatedAtAction(nameof(GetAllAccounts), new { id = response.Data }, response);
        }

        [HttpGet]
        [Route("GetAllAccounts")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllAccounts()
        {
            var accounts = await _dashboardService.GetCompanyAccountsAsync();
            if (accounts == null || accounts.Data == null || !accounts.Data.Any())
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = "No account found.",
                    Errors = new List<string> { "Account list is empty." }
                });
            }

            return Ok(new Response<IEnumerable<CompanyAccounts>>
            {
                Success = true,
                Message = "Accounts retrieved successfully.",
                Data = accounts.Data
            });
        }

        [HttpDelete("DeleteAccount/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var result = await _dashboardService.DeleteAccountsAsync(id);
            if (!result.Success)
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = result.Message,
                    Errors = new List<string> { "Failed to delete account." }
                });
            }

            return Ok(result.Message);
        }
    }
}

