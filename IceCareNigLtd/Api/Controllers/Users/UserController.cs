using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Network;
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


        // User Registration Endpoint
        [HttpPost]
        [Route("Register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

                return CreatedAtAction(nameof(Register), new { id = result.Data.Id }, result.Data);
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





        // User Login Endpoint
        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null)
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
                    Message = "Login failed.",
                    Errors = new List<string> { "Invalid credentials or user not approved." }
                });
            }

            return Ok(result.Data);
        }
    }
}

