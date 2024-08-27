using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Network;
using IceCareNigLtd.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IceCareNigLtd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }


        [HttpPost]
        [Route("AddPaymentRecord")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddPayment([FromBody] PaymentDto paymentDto)
        {
            if (paymentDto == null)
            {
                return BadRequest("Payment data cannot be null.");
            }

            var requiredFields = new Dictionary<string, string>
            {
                { nameof(paymentDto.CustomerName), paymentDto.CustomerName },
                { nameof(paymentDto.ModeOfPayment), paymentDto.ModeOfPayment},
                { nameof(paymentDto.DollarAmount), paymentDto.DollarAmount.ToString() }
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

            var response = await _paymentService.AddPaymentAsync(paymentDto);

            if (!response.Success)
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = response.Message,
                    Errors = new List<string> { "Failed to add payment." }
                });
            }

            return CreatedAtAction(nameof(GetPayments), new { id = response.Data }, response);
        }



        [HttpGet]
        [Route("GetPaymentsRecord")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPayments()
        {
            var response = await _paymentService.GetPaymentsAsync();

            // Check if the response or its data is null
            if (response == null || response.Data == null || !response.Data.Any())
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = "No payments found.",
                    Errors = new List<string> { "Payment list is empty." }
                });
            }

            return Ok(new Response<IEnumerable<PaymentDto>>
            {
                Success = true,
                Message = "Payments retrieved successfully.",
                Data = response.Data
            });
        }
    }
}

