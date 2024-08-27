using System;
using System.Net.NetworkInformation;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Response;
using IceCareNigLtd.Api.Models.Users;
using IceCareNigLtd.Core.Entities.Users;
using IceCareNigLtd.Core.Interfaces;
using IceCareNigLtd.Core.Interfaces.Users;
using IceCareNigLtd.Infrastructure.Interfaces;
using IceCareNigLtd.Infrastructure.Interfaces.Users;
using Microsoft.AspNetCore.Identity;

namespace IceCareNigLtd.Core.Services.Users
{
	public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ISettingsRepository _settingsRepository;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, ISettingsRepository settingsRepository)
		{
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _settingsRepository = settingsRepository;
        }

        public async Task<Response<Registration>> RegisterUserAsync(RegistrationDto registrationDto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(registrationDto.Email);
            var phoneNumberExists = await _userRepository.IsPhoneNumberExistsAsync(registrationDto.Phone);

            if (existingUser != null)
            {
                return new Response<Registration>
                {
                    Success = false,
                    Message = "Email already in use."
                };
            }
            else if (phoneNumberExists)
            {
                return new Response<Registration>
                {
                    Success = false,
                    Message = "Phone number already in use."
                };
            }

            var hashedPassword = _passwordHasher.HashPassword(registrationDto.Password);

            var newUser = new Registration
            {
                FullName = registrationDto.FullName,
                Email = registrationDto.Email,
                Phone = registrationDto.Phone,
                Password = hashedPassword,
                Date = DateTime.UtcNow,
                Status = "Pending",
                AccountNumber = "",
                Reason = "",
                ReviewedBy = ""
            };

            await _userRepository.AddUserAsync(newUser);

            return new Response<Registration>
            {
                Success = true,
                Message = "User registered successfully.",
                Data = newUser
            };
        }

        public async Task<Response<LoginResponse>> LoginUserAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
            // Get current Dollar Rate
            var dollarRate = await _settingsRepository.GetDollarRateAsync();
            var companyPhone = await _settingsRepository.GetCompanyPhoneNumbersAsync();
            var companyAccounts = await _settingsRepository.GetCompanyAccountsAsync();

            if (user == null || !_passwordHasher.VerifyPassword(user.Password, loginDto.Password))
            {
                return new Response<LoginResponse>
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }

            if (user.Status != "Approved")
            {
                return new Response<LoginResponse>
                {
                    Success = false,
                    Message = "User not approved by admin."
                };
            }

            var userDto = new LoginResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Status = user.Status,
                AccountNumber = user.AccountNumber,
                Phone = user.Phone,
                AccountBalance  = "",
                CompanyNumber = companyPhone,
                DollarRate = dollarRate,
                CompanyAccounts = companyAccounts
            };

            return new Response<LoginResponse>
            {
                Success = true,
                Message = "Login successful.",
                Data = userDto
            };
        }
    }
}

