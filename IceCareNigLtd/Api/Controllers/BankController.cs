using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Network;
using IceCareNigLtd.Core.Interfaces;
using IceCareNigLtd.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IceCareNigLtd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly IBankService _bankService;

        public BankController(IBankService bankService)
        {
            _bankService = bankService;
        }

        [HttpPost]
        [Route("AddBankRecord")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddBankRecord([FromBody] BankDto bankDto)
        {
            if (bankDto == null)
            {
                return BadRequest("Bank data cannot be null.");
            }

            var requiredFields = new Dictionary<string, string>
            {
                { nameof(bankDto.BankName), bankDto.BankName },
                { nameof(bankDto.Amount), bankDto.Amount.ToString() }
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

            var response = await _bankService.AddBankAsync(bankDto);

            if (!response.Success)
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = response.Message,
                    Errors = new List<string> { "Failed to add bank record." }
                });
            }

            return CreatedAtAction(nameof(GetBankRecords), new { id = response.Data }, response);
        }



        [HttpGet]
        [Route("GetBanksRecord")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBankRecords()
        {
            var banks = await _bankService.GetBanksAsync();
            if (banks == null || banks.Data == null || !banks.Data.Any())
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = "No bank records found.",
                    Errors = new List<string> { "Bank records list is empty." }
                });
            }

            return Ok(new Response<IEnumerable<BankDto>>
            {
                Success = true,
                Message = "Bank records retrieved successfully.",
                Data = banks.Data
            });
        }
    }
}

