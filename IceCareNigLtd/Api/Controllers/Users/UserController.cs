using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Network;
using IceCareNigLtd.Api.Models.Request;
using IceCareNigLtd.Api.Models.Response;
using IceCareNigLtd.Api.Models.Users;
using IceCareNigLtd.Core.Interfaces.Users;
using IceCareNigLtd.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IceCareNigLtd.Api.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IUserService _userService;

		public UserController(IUserService userService)
		{
            _userService = userService;
		}

        [HttpPost]
        [Route("Register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<string>), 200)]
        public async Task<IActionResult> Register([FromBody] RegistrationDto registrationDto)
        {
            if (registrationDto == null)
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = "Registration data cannot be null.",
                    Errors = new List<string> { "Invalid input." }
                });
            }

            var requiredFields = new Dictionary<string, string>
            {
                { nameof(registrationDto.FullName), registrationDto.FullName },
                { nameof(registrationDto.Email), registrationDto.Email },
                { nameof(registrationDto.Phone), registrationDto.Phone },
                { nameof(registrationDto.Password), registrationDto.Password }
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

            if (!Helpers.IsValidEmail(registrationDto.Email))
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = "Invalid email format.",
                    Errors = new List<string> { "Invalid email format." }
                });
            }

            if (!Helpers.IsValidPhoneNumber(registrationDto.Phone))
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = "Invalid phone number format.",
                    Errors = new List<string> { "Invalid phone number format." }
                });
            }

            try
            {
                var result = await _userService.RegisterUserAsync(registrationDto);
                if (!result.Success)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Success = false,
                        Message = result.Message,
                        Errors = new List<string> { "Failed to register user." }
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    Success = false,
                    Message = "An unexpected error occurred.",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Response<LoginResponse>), 200)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = "Login data cannot be null.",
                    Errors = new List<string> { "Invalid input." }
                });
            }

            var result = await _userService.LoginUserAsync(loginDto);

            if (!result.Success)
            {
                return Unauthorized(new ErrorResponse
                {
                    Success = false,
                    Message = result.Message,
                    Errors = new List<string> { result.Message }
                });
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        [Authorize]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetPasswordRequest)
        {
            var requiredFields = new Dictionary<string, string>
            {
                { nameof(resetPasswordRequest.FullName), resetPasswordRequest.FullName },
                { nameof(resetPasswordRequest.Email), resetPasswordRequest.Email },
                { nameof(resetPasswordRequest.Phone), resetPasswordRequest.Phone },
                { nameof(resetPasswordRequest.NewPassword), resetPasswordRequest.NewPassword },
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

            var response = await _userService.ResetUserLoginAsync(resetPasswordRequest);
            if (!response.Success)
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = response.Message
                });
            }
            return Ok(response.Data);
        }


        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        [Route("FundTransfer")]
        public async Task<IActionResult> FundTransfer([FromBody] TransferRequest transferRequest)
        {
            if (string.IsNullOrEmpty(transferRequest.DollarAmount.ToString()))
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = "Dollar amount cannot be empty.",
                    Errors = new List<string> { "Invalid input." }
                });
            }
            if (!transferRequest.BankDetails.Any() || transferRequest.BankDetails == null)
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = "Dollar amount cannot be empty.",
                    Errors = new List<string> { "Invalid input." }
                });
            }
            if (string.IsNullOrEmpty(transferRequest.BankDetails[0].BankName.ToString()))
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = "Bank Name cannot be empty.",
                    Errors = new List<string> { "Invalid input." }
                });
            }
            if (string.IsNullOrEmpty(transferRequest.BankDetails[0].TransferredAmount.ToString()))
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = "Transferred amount cannot be empty.",
                    Errors = new List<string> { "Invalid input." }
                });
            }
            if (string.IsNullOrEmpty(transferRequest.TransferEvidence[0].Receipts.ToString()))
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = "Receipt must be provided",
                    Errors = new List<string> { "Invalid input." }
                });
            }

            var response = await _userService.FundTransferAsync(transferRequest);
            if (!response.Success)
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = response.Message,
                    Data = false
                });
            }
            return Ok(response);
        }


        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        [Route("AccountBalancePayment")]
        public async Task<IActionResult> AccountPayment([FromBody] AccountPaymentRequest accountPaymentRequest)
        {
            if (string.IsNullOrEmpty(accountPaymentRequest.Amount.ToString()))
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = "Amount cannot be empty.",
                    Errors = new List<string> { "Invalid input." }
                });
            }

            var response = await _userService.AccountPaymentAsync(accountPaymentRequest);
            if (!response.Success)
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = response.Message,
                    Data = false
                });
            }
            return Ok(response);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        [Route("ThirdPartyPayment")]
        public async Task<IActionResult> ThirdPartyPayment([FromBody] ThirdPartyPaymentRequest thirdPartyPaymentRequest)
        {
            var requiredFields = new Dictionary<string, string>
            {
                { nameof(thirdPartyPaymentRequest.AccountName), thirdPartyPaymentRequest.AccountName },
                { nameof(thirdPartyPaymentRequest.AccountNumber), thirdPartyPaymentRequest.AccountNumber },
                { nameof(thirdPartyPaymentRequest.BankName), thirdPartyPaymentRequest.BankName },
                { nameof(thirdPartyPaymentRequest.Amount), thirdPartyPaymentRequest.Amount.ToString() },
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

            var response = await _userService.ThirdPartyPaymentAsync(thirdPartyPaymentRequest);
            if (!response.Success)
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = response.Message,
                    Data = false
                });
            }
            return Ok(response);
        }


        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        [Route("AccountTopUp")]
        public async Task<IActionResult> AccountTopUp([FromBody] AccoutTopUpRequest request)
        {
            var requiredFields = new Dictionary<string, string>
            {
                { nameof(request.Email), request.Email },
                { nameof(request.Phone), request.Phone }
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

            if (request.BankDetails.Count > request.TransferEvidence.Count)
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = $"You are expected to upload {request.BankDetails.Count} receipt(s) for all bank transfer",
                    Data = false
                });
            }

            var response = await _userService.TopUpAccountAsync(request);
            if (!response.Success)
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = response.Message,
                    Data = false
                });
            }
            return Ok(response);
        }


        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<UserAccount>), 200)]
        [Route("RefreshAccount")]
        public async Task<IActionResult> RefreshAccount(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest(new ErrorResponse { Success = false, Message = "No email passed", Data = "failed to proceed" });

            var result = await _userService.RefreshAccount(email);
            if (result.Data != null)
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = "No record found",
                });
            };
            return Ok(result);
        }


        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<bool>), 200)]
        [Route("CheckTransferStatus")]
        public async Task<IActionResult> CheckTransferStatus(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest(new ErrorResponse { Success = false, Message = "No email passed", Data = "failed to proceed"});

            var result = await _userService.GetTransferStatus(email);
            if (!result.Data)
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = "No record found",
                });
            };
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<List<TransactionHistoryResponse>>), 200)]
        [Route("GetTransactionHistory")]
        public async Task<IActionResult> GetTransactionHistory(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest(new ErrorResponse { Success = false, Message = "No email passed", Data = "failed to proceed" });

            var result = await _userService.GetTransactionHistory(email);
            if (result.Data == null || !result.Data.Any())
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = "No transaction history found.",
                    Errors = new List<string> { "List is empty." }
                });
            };
            return Ok(result);
        }
    }
}

