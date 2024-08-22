using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Network;
using IceCareNigLtd.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace IceCareNigLtd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost]
        [Route("AddAdmin")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddAdmin([FromBody] AdminDto adminDto)
        {
            if (adminDto == null)
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = "Admin data cannot be null.",
                    Errors = new List<string> { "Invalid input." }
                });
            }

            if (string.IsNullOrEmpty(adminDto.Email) || string.IsNullOrEmpty(adminDto.Password))
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = "Email and Password cannot be null or empty.",
                    Errors = new List<string> { "Invalid input." }
                });
            }

            var response = await _adminService.AddAdminAsync(adminDto);

            if (!response.Success)
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = response.Message,
                    Errors = new List<string> { "Failed to add admin." }
                });
            }

            return CreatedAtAction(nameof(GetAdmins), new { id = response.Data }, response);
        }


        [HttpGet]
        [Route("GetAllAdmins")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAdmins()
        {
            var admins = await _adminService.GetAdminsAsync();
            if (admins == null || admins.Data == null || !admins.Data.Any())
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = "No admins found.",
                    Errors = new List<string> { "Admin list is empty." }
                });
            }

            return Ok(new Response<IEnumerable<AdminDto>>
            {
                Success = true,
                Message = "Admins retrieved successfully.",
                Data = admins.Data
            });
        }



        [HttpDelete("DeleteAdmin/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var result = await _adminService.DeleteAdminAsync(id);
            if (!result.Success)
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = result.Message,
                    Errors = new List<string> { "Failed to delete admin." }
                });
            }

            return NoContent();
        }



        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] AdminLoginDto loginDto)
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

            var response = await _adminService.LoginAsync(loginDto);

            if (!response.Success)
            {
                return Unauthorized(new ErrorResponse
                {
                    Success = false,
                    Message = response.Message,
                    Errors = new List<string> { "Authentication failed." }
                });
            }

            return Ok(response);
        }
    }
}

