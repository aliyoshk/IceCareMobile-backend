using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Network;
using IceCareNigLtd.Api.Models.Request;
using IceCareNigLtd.Api.Models.Response;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Core.Interfaces;
using IceCareNigLtd.Core.Services;
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
                { nameof(customerDto.DollarRate), customerDto.DollarRate.ToString() },
                { nameof(customerDto.PaymentCurrency), customerDto.PaymentCurrency },
                { nameof(customerDto.Balance), customerDto.Balance.ToString() },
                { nameof(customerDto.PhoneNumber), customerDto.PhoneNumber.ToString() },
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

        [HttpDelete("DeleteCustomer/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);
            if (!result.Success)
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = result.Message,
                    Errors = new List<string> { "Failed to customer admin." }
                });
            }

            return Ok(result);
        }


        [HttpPost]
        [Route("CompleteCustomerPayment")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CompleteCustomerPayment([FromBody] CompletePaymentRequest completePaymentRequest)
        {
            if (completePaymentRequest == null)
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
                { nameof(completePaymentRequest.CustomerId), completePaymentRequest.CustomerId.ToString() },
                { nameof(completePaymentRequest.DollarAmount), completePaymentRequest.DollarAmount.ToString()},
                { nameof(completePaymentRequest.Charges), completePaymentRequest.Charges.ToString() },
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

            var response = await _customerService.CompleteCustomerPayment(completePaymentRequest);

            if (!response.Success)
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = response.Message,
                    Errors = new List<string> { "Failed to complete customer payment request." }
                });
            }

            return Ok(response);
        }
    }
}

