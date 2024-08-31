using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Network;
using IceCareNigLtd.Api.Models.Request;
using IceCareNigLtd.Api.Models.Response;
using IceCareNigLtd.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }


        [HttpPost]
        [Route("AddSupplier")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddSupplier([FromBody] SupplierRequest supplierDto)
        {
            if (supplierDto == null || string.IsNullOrEmpty(supplierDto.Name) || supplierDto.Banks == null || !supplierDto.Banks.Any())
            {
                return BadRequest("Invalid supplier data.");
            }

            var requiredFields = new Dictionary<string, string>
            {
                { nameof(supplierDto.Name), supplierDto.Name },
                { nameof(supplierDto.DollarAmount), supplierDto.DollarAmount.ToString()},
                { nameof(supplierDto.DollarRate), supplierDto.DollarRate.ToString() },
                { nameof(supplierDto.ModeOfPayment), supplierDto.ModeOfPayment}
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

            if (supplierDto.ModeOfPayment == ModeOfPayment.Transfer.ToString() && supplierDto.Banks[0].AmountTransferred <= 0)
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = "Banks details cannot be null",
                    Errors = new List<string> { "Invalid input." }
                });
            }

            var response = await _supplierService.AddSupplierAsync(supplierDto);

            if (!response.Success)
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = response.Message,
                    Errors = new List<string> { "Failed to add supplier." }
                });
            }

            return CreatedAtAction(nameof(GetSuppliers), new { id = response.Data }, response);
        }



        [HttpGet]
        [Route("GetSuppliersRecord")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSuppliers()
        {
            var response = await _supplierService.GetSuppliersAsync();

            if (!response.Data.Suppliers.Any())
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = "No suppliers found.",
                    Errors = new List<string> { "Supplier list is empty." }
                });
            }
            return Ok(response.Data);
        }
    }
}

