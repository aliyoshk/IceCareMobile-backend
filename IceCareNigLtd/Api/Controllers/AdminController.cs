using System;
using System.Security.Claims;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Network;
using IceCareNigLtd.Api.Models.Request;
using IceCareNigLtd.Api.Models.Response;
using IceCareNigLtd.Api.Models.Users;
using IceCareNigLtd.Core.Interfaces;
using IceCareNigLtd.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static IceCareNigLtd.Core.Enums.Enums;

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
        [Route("AdminLogin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(Response<AdminLoginDto>), 200)]
        [Produces("application/json")]
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

        [HttpPost]
        [Route("AddAdmin")]
        //[Authorize(Policy = "AdminOnly")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Response<List<AdminDto>>), 200)]
        [Produces("application/json")]
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

            var requiredFields = new Dictionary<string, string>
            {
                { nameof(adminDto.Name), adminDto.Name },
                { nameof(adminDto.Email), adminDto.Email },
                { nameof(adminDto.Password), adminDto.Password }
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

            if (!Helpers.IsValidEmail(adminDto.Email))
            {
                return BadRequest(new ErrorResponse
                {
                    Success = false,
                    Message = "Invalid email format.",
                    Errors = new List<string> { "Invalid email format." }
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
        [ProducesResponseType(typeof(Response<List<AdminDto>>), 200)]
        [Produces("application/json")]
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
        //[Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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

            return Ok(result);
        }


        // MOBILE PART INTEGRATION
        // Get Pending Registrations
        [HttpGet]
        [Authorize]
        [Route("PendingRegistrations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Response<List<UserDto>>), 200)]
        [Produces("application/json")]
        public async Task<IActionResult> GetPendingRegistrations()
        {
            var pendingUsers = await _adminService.GetUsersByStatusAsync("Pending");
            if (pendingUsers.Data == null || !pendingUsers.Data.Any())
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = "No record found.",
                    Errors = new List<string> { "List is empty." }
                });
            };
            return Ok(pendingUsers);
        }


        [HttpPost]
        [Authorize]
        [Route("ChangeUserStatus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response<ChangeUserStatusRequest>), 200)]
        [Produces("application/json")]
        public async Task<IActionResult> ChangeUserStatus([FromBody] ChangeUserStatusRequest request)
        {
            // Get admin's name from the current user
            var adminName = User.Identity.Name ?? User.FindFirst(ClaimTypes.Name)?.Value;

            // Call the service method with the request object and adminName
            var result = await _adminService.ChangeUserStatusAsync(request, adminName);
            if (!result.Success)
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = result.Message,
                    Errors = new List<string> { "User not found or invalid action." }
                });
            }

            return Ok(result);
        }


        // Get Approved Users
        [HttpGet]
        [Authorize]
        [Route("ApprovedUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response<List<UserDto>>), 200)]
        [Produces("application/json")]
        public async Task<IActionResult> GetApprovedUsers()
        {
            var result = await _adminService.GetApprovedUsersAsync();
            if (result.Data == null || !result.Data.Any())
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = "No record found.",
                    Errors = new List<string> { "List is empty." }
                });
            };
            return Ok(result);
        }

        // Get Rejected Users
        [HttpGet]
        [Authorize]
        [Route("RejectedUsers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response<List<UserDto>>), 200)]
        [Produces("application/json")]
        public async Task<IActionResult> GetRejectedUsers()
        {
            var result = await _adminService.GetRejectedUsersAsync();
            if (result.Data == null || !result.Data.Any())
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = "No record found.",
                    Errors = new List<string> { "List is empty." }
                });
            };
            return Ok(result);
        }

        [HttpDelete("DeleteUser/{id}")]
        //[Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response<object>), 200)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _adminService.DeleteUserAsync(id);
            if (!result.Success)
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = result.Message,
                    Errors = new List<string> { "Failed to delete user." }
                });
            }

            return Ok(result);
        }


        [HttpGet]
        [Authorize]
        [Route("GetPendingTransfer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response<List<TransferResponse>>), 200)]
        [Produces("application/json")]
        public async Task<IActionResult> GetPendingTransfer()
        {
            var result = await _adminService.GetUsersByTransferStatusAsync("Pending");
            if (result.Data == null || !result.Data.Any())
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = "No record found.",
                    Errors = new List<string> { "List is empty." }
                });
            };
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [Route("GetApprovedTransfer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response<List<TransferResponse>>), 200)]
        [Produces("application/json")]
        public async Task<IActionResult> GetApprovedTransfer()
        {
            var result = await _adminService.GetUsersByTransferStatusAsync("Confirmed");
            if (result.Data == null || !result.Data.Any())
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = "No record found.",
                    Errors = new List<string> { "List is empty." }
                });
            };
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [Route("ConfirmTransfer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response<ConfirmationRequest>), 200)]
        [Produces("application/json")]
        public async Task<IActionResult> ConfirmTransfer([FromBody] ConfirmationRequest request)
        {
            // Get admin's name from the current user
            var adminName = User.Identity.Name ?? User.FindFirst(ClaimTypes.Name)?.Value;

            // Call the service method with the request object and adminName
            var result = await _adminService.ConfirmTransferAsync(request, adminName);
            if (!result.Success)
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = result.Message,
                    Errors = new List<string> { "User not found or invalid action." }
                });
            }

            return Ok(result);
        }


        [HttpGet]
        [Authorize]
        [Route("GetThirdPartyTransfers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response<List<ThirdPartyPaymentResponse>>), 200)]
        [Produces("application/json")]
        public async Task<IActionResult> GetThirdPartyTransfers()
        {
            var result = await _adminService.GetThirdPartyTransfer();
            if (result.Data == null || !result.Data.Any())
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = "No record found.",
                    Errors = new List<string> { "List is empty." }
                });
            };
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [Route("CompleteThirdPartyTransfer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ThirdPartyTransferCompleted(int id)
        {
            var result = await _adminService.ThirdPartyTransferCompleted(id);
            if (!result.Success)
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = result.Message,
                    Errors = new List<string> { "Failed to approved transfer." }
                });
            }

            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [Route("GetAccountTopUps")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Response<List<ThirdPartyPaymentResponse>>), 200)]
        [Produces("application/json")]
        public async Task<IActionResult> GetAccountTopUps()
        {
            var result = await _adminService.GetAccountTopUpsAsync();
            if (result.Data == null || !result.Data.Any())
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = "No record found.",
                    Errors = new List<string> { "List is empty." }
                });
            };
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [Route("ConfirmAccountTopUp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConfirmAccountTopUp(ConfirmationRequest request)
        {
            var result = await _adminService.ConfirmAccountTopUp(request);
            if (!result.Success)
            {
                return NotFound(new ErrorResponse
                {
                    Success = false,
                    Message = result.Message,
                    Errors = new List<string> { "Failed to approved account top up." }
                });
            }

            return Ok(result);
        }
    }
}

