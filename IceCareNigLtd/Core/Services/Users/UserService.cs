﻿using System;
using System.Net.NetworkInformation;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Request;
using IceCareNigLtd.Api.Models.Response;
using IceCareNigLtd.Api.Models.Users;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Core.Entities.Users;
using IceCareNigLtd.Core.Interfaces;
using IceCareNigLtd.Core.Interfaces.Users;
using IceCareNigLtd.Infrastructure.Interfaces;
using IceCareNigLtd.Infrastructure.Interfaces.Users;
using IceCareNigLtd.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Core.Services.Users
{
	public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ISettingsRepository _settingsRepository;
        private readonly ICustomerRepository _customerRepository;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, ISettingsRepository settingsRepository,
             ICustomerRepository customerRepository)
		{
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _settingsRepository = settingsRepository;
            _customerRepository = customerRepository;
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
            var customer = await _customerRepository.GetCustomerByIdAsync(user.Id);
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
                AccountBalance  = customer.Balance.ToString() ?? 0.ToString(),
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

        public async Task<Response<bool>> ResetUserLoginAsync(ResetPasswordRequest resetPasswordRequest)
        {
            var user = await _userRepository.GetUserByEmailAsync(resetPasswordRequest.Email);
            if (user == null)
            {
                return new Response<bool>
                {
                    Success = false,
                    Message = "Email doesn't exist",
                    Data = false
                };
            }
            if (user.Phone != resetPasswordRequest.Phone)
            {
                return new Response<bool>
                {
                    Success = false,
                    Message = "The phone number does not match our records.",
                    Data = false
                };
            }
            if(user.FullName.ToLower() != resetPasswordRequest.FullName.ToLower())
            {
                return new Response<bool>
                {
                    Success = false,
                    Message = "The Name does not correspond with enterred data.",
                    Data = false
                };
            }
            if (user.Status != "Approved")
            {
                return new Response<bool>
                {
                    Success = false,
                    Message = "User profile not approved by admin, password cannot be reset",
                    Data = false
                };
            }

            var hashedPassword = _passwordHasher.HashPassword(resetPasswordRequest.NewPassword);
            user.Password = hashedPassword;

            await _userRepository.ResetPasswordAsync(user);

            return new Response<bool>
            {
                Success = true,
                Message = "Password reset successfully.",
                Data = true
            };
        }

        public async Task<Response<bool>> FundTransferAsync(TransferRequest transferRequest)
        {
            var user = await _userRepository.GetUserByEmailAsync(transferRequest.CustomerEmail);
            if (user == null)
            {
                return new Response<bool> { Success = false, Message = "Email address is null", Data = false };
            }
            var customer = await _customerRepository.GetCustomerByIdAsync(user.Id);
            if (customer == null)
            {
                return new Response<bool> { Success = false, Message = "Customer not found", Data = false };
            }

            var data = new Transfer
            {
                TransactionDate = DateTime.Now,
                DollarAmount = transferRequest.DollarAmount,
                Description = transferRequest.Description,
                Channel = Channel.Mobile,
                CustomerAccount = customer.AccountNumber,
                Balance = customer.Balance,
                CustomerName = customer.Name,
                BankDetails = transferRequest.BankDetails.Select(b => new TransferBank
                {
                    TransferredAmount = b.TransferredAmount,
                    BankName = Enum.Parse<BankName>(b.BankName.ToString()),
                }).ToList(),
                TransferEvidence = transferRequest.TransferEvidence.Select(e => new EvidenceOfTransfer
                {
                    Receipts = e.Receipts
                }).ToList()
            };

            await _userRepository.FundTransferAsync(data);

            return new Response<bool>
            {
                Success = true,
                Message = "You transfer details has been successfully submitted for admin to verified. " +
                "You will be notified once the transfer is confirmed.\n\nYou can confirmed the status of " +
                "your transfer in the dashboard\n"
            };
        }

        public async Task<Response<bool>> AccountPaymentAsync(AccountPaymentRequest accountPaymentRequest)
        {
            var user = await _userRepository.GetUserByEmailAsync(accountPaymentRequest.CustomerEmail);
            if (user == null)
            {
                return new Response<bool> { Success = false, Message = "Email address is null", Data = false };
            }
            var customer = await _customerRepository.GetCustomerByIdAsync(user.Id);
            if (customer == null)
            {
                return new Response<bool> { Success = false, Message = "Customer not found", Data = false };
            }

            var data = new AccountPayment
            {
                NairaAmount = accountPaymentRequest.NairaAmount,
                DollarAmount = accountPaymentRequest.DollarAmount,
                Description = accountPaymentRequest.Description,
                CustomerAccount = customer.AccountNumber,
                Balance = customer.Balance,
                CustomerName = customer.Name,
                Channel = Channel.None
            };

            await _userRepository.AccountPaymentAsync(data);
            return new Response<bool>
            {
                Success = true,
                Message = "Your request has been successfully submitted for admin to verified. You will be notified once confirmed" +
                ".\n\nYou can confirmed the status of your transfer in the dashboard\n"
            };
        }

        public async Task<Response<bool>> ThirdPartyPaymentAsync(ThirdPartyPaymentRequest thirdPartyPaymentRequest)
        {
            var user = await _userRepository.GetUserByEmailAsync(thirdPartyPaymentRequest.CustomerEmail);
            if (user == null)
            {
                return new Response<bool> { Success = false, Message = "Email address is null", Data = false };
            }
            var customer = await _customerRepository.GetCustomerByIdAsync(user.Id);
            if (customer == null)
            {
                return new Response<bool> { Success = false, Message = "Customer not found", Data = false };
            }

            var data = new ThirdPartyPayment
            {
                Amount = thirdPartyPaymentRequest.Amount,
                AccountName = thirdPartyPaymentRequest.AccountName,
                AccountNumber = thirdPartyPaymentRequest.AccountNumber,
                BankName = thirdPartyPaymentRequest.BankName,
                Description = thirdPartyPaymentRequest.Description,
                CustomerAccount = customer.AccountNumber,
                Balance = customer.Balance,
                CustomerName = customer.Name,
                Channel = Channel.None
            };

            await _userRepository.ThirdPartyPaymentAsync(data);
            return new Response<bool>
            {
                Success = true,
                Message = " Your transfer details has been successful submitted\nYou’ll be notified once the transfer has been made."
            };
        }
    }
}

