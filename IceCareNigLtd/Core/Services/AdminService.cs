using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Core.Interfaces;
using IceCareNigLtd.Infrastructure.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace IceCareNigLtd.Core.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public AdminService(IAdminRepository adminRepository, ITokenService tokenService, IConfiguration configuration)
        {
            _adminRepository = adminRepository;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        public async Task<Response<AdminDto>> AddAdminAsync(AdminDto adminDto)
        {
            // Check if an admin with the same email already exists
            var existingAdmin = await _adminRepository.GetAdminByUsernameAsync(adminDto.Email);
            if (existingAdmin != null)
            {
                return new Response<AdminDto>
                {
                    Success = false,
                    Message = "An admin with this email already exists",
                    Data = null
                };
            }
            var admin = new Admin
            {
                Name = adminDto.Name,
                Email = adminDto.Email,
                Password = adminDto.Password, // Password should be hashed in production
                Role = "Normal",
                Date = DateTime.UtcNow
            };

            await _adminRepository.AddAdminAsync(admin);

            return new Response<AdminDto> { Success = true, Message = "Admin added successfully", Data = adminDto };
        }

        public async Task<Response<List<AdminDto>>> GetAdminsAsync()
        {
            var admins = await _adminRepository.GetAdminsAsync();
            var adminDtos = admins.Select(a => new AdminDto
            {
                Name = a.Name,
                Email = a.Email,
                Role = a.Role,
                Date = a.Date,
                Password = "*********"
            }).ToList();

            return new Response<List<AdminDto>> { Success = true, Message = "Admins retrieved successfully", Data = adminDtos };
        }

        public async Task<Response<object>> DeleteAdminAsync(int adminId)
        {
            await _adminRepository.DeleteAdminAsync(adminId);
            return new Response<object> { Success = true, Message = "Admin deleted successfully" };
        }


        public async Task<Response<string>> LoginAsync(AdminLoginDto adminLoginDto)
        {
            if (adminLoginDto == null)
            {
                return new Response<string> { Success = false, Message = "Login data cannot be null" };
            }

            // Check against hardcoded credentials
            //var hardcodedEmail = "aliyoshk@gmail.com";
            //var hardcodedPassword = "1234";
            //if (adminLoginDto.Email == hardcodedEmail && adminLoginDto.Password == hardcodedPassword)
            //{
            //    var token = _tokenService.GenerateToken(hardcodedEmail);
            //    return new Response<string> { Success = true, Message = "Login successful", Data = token };
            //}

            // Check against credentials stored in the database
            var admin = await _adminRepository.GetAdminByUsernameAsync(adminLoginDto.Email);

            if (admin != null && admin.Password == adminLoginDto.Password) // Verify hashed password in a real scenario
            {
                var token = _tokenService.GenerateToken(admin.Email);
                return new Response<string> { Success = true, Message = "Login successful", Data = token };
            }

            return new Response<string> { Success = false, Message = "Invalid credentials" };
        }
    }
}


