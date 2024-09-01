﻿using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Network;
using IceCareNigLtd.Api.Models.Response;
using IceCareNigLtd.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }


        [HttpPost]
        [Route("AddCustomer")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddCustomer([FromBody] CustomerDto customerDto)
        {
            if (customerDto == null)
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = "Customer data cannot be null.",
                    Errors = new List<string> { "Invalid input." }
                });
            }

            var requiredFields = new Dictionary<string, string>
            {
                { nameof(customerDto.Name), customerDto.Name },
                { nameof(customerDto.ModeOfPayment), customerDto.ModeOfPayment},
                { nameof(customerDto.DollarAmount), customerDto.DollarAmount.ToString() },
                { nameof(customerDto.DollarRate), customerDto.DollarRate.ToString() },
                { nameof(customerDto.PaymentCurrency), customerDto.PaymentCurrency },
                { nameof(customerDto.Balance), customerDto.Balance.ToString() },
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

            var response = await _customerService.AddCustomerAsync(customerDto);

            if (!response.Success)
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = response.Message,
                    Errors = new List<string> { "Failed to add customer." }
                });
            }

            return CreatedAtAction(nameof(GetCustomers), new { id = response.Data }, response);
        }



        [HttpGet]
        [Route("GetCustomersRecord")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _customerService.GetCustomersAsync();

            if (!customers.Data.Customers.Any())
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = "No Customer found.",
                    Errors = new List<string> { "Customer list is empty." }
                });
            }
            return Ok(customers.Data);
        }
    }
}

