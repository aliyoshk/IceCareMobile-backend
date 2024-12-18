using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
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
        private readonly IBankRepository _bankRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ITokenService _tokenService;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, ISettingsRepository settingsRepository,
             ICustomerRepository customerRepository, IBankRepository bankRepository, ISupplierRepository supplierRepository,
             ITokenService tokenService)
		{
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _settingsRepository = settingsRepository;
            _customerRepository = customerRepository;
            _bankRepository = bankRepository;
            _supplierRepository = supplierRepository;
            _tokenService = tokenService;
        }

        public async Task<Response<LoginResponse>> LoginUserAsync(LoginDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                return new Response<LoginResponse> { Success = false, Message = "Login details cannot be empty" };

            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);

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

            var customer = await _customerRepository.GetCustomerByIdAsync(user.Id);
            // Get current Dollar Rate
            var dollarRate = await _settingsRepository.GetDollarRateAsync();
            var companyPhone = await _settingsRepository.GetCompanyPhoneNumbersAsync();
            var companyAccounts = await _settingsRepository.GetCompanyAccountsAsync();

            var balance = 0.0m;
            if (customer != null)
                balance = customer.Balance;

            var token = _tokenService.GenerateToken(user.Email, user.FullName);

            var userDto = new LoginResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Token = token,
                Status = user.Status,
                AccountNumber = user.AccountNumber,
                Phone = user.Phone,
                AccountBalance = balance.ToString("F2"),
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

        public async Task<Response<string>> RegisterUserAsync(RegistrationDto registrationDto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(registrationDto.Email);
            var phoneNumberExists = await _userRepository.IsPhoneNumberExistsAsync(registrationDto.Phone);

            if (existingUser != null)
            {
                return new Response<string>
                {
                    Success = false,
                    Message = "Email already in use",
                    Data = "Email already in use"

                };
            }
            else if (phoneNumberExists)
            {
                return new Response<string>
                {
                    Success = false,
                    Message = "Phone number already in use",
                    Data = "Phone number already in use."
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

            return new Response<string>
            {
                Success = true,
                Message = "Success",
                Data = "User registered successfully.",
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
            var totalSupplierDollarAmount = await _supplierRepository.GetTotalDollarAmountAsync();
            if (totalSupplierDollarAmount < transferRequest.DollarAmount)
            {
                return new Response<bool>
                {
                    Success = false,
                    Message = "Request can not be completed, Please try again later",
                    Data = true
                };
            }

            var user = await _userRepository.GetUserByEmailAsync(transferRequest.CustomerEmail);
            if (user == null)
                return new Response<bool> { Success = false, Message = "Email address is null", Data = false };

            var accounts = await _settingsRepository.GetCompanyAccountsAsync();
            if (!accounts.Any())
                return new Response<bool> { Success = false, Message = "Company bank(s) detail(s) is/are null" };


            var companyBankNames = accounts.Select(a => a.BankName.ToLower().Trim()).ToList();
            foreach (var bankRequest in transferRequest.BankDetails)
            {
                if (!companyBankNames.Contains(bankRequest.BankName.ToLower().Trim()))
                    return new Response<bool> { Success = false, Message = $"Bank '{bankRequest.BankName}' doesn't exist in the system." };
            }


            Category transactionCategory = Category.SingleBankPayment;
            if (transferRequest.BankDetails.Count > 1)
                transactionCategory = Category.MultipleBanksPayment;

            var transactionReference = await GenerateTransactionReference();
            var data = new Transfer
            {
                TransactionDate = DateTime.UtcNow,
                DollarAmount = transferRequest.DollarAmount,
                Description = transferRequest.Description,
                Channel = Channel.Mobile,
                CustomerAccount = user.AccountNumber,
                Balance = 0,
                CustomerName = user.FullName,
                Email = user.Email,
                DollarRate = transferRequest.DollarRate,
                TransferReference = transactionReference,
                Status = "Pending",
                Approver = user.ReviewedBy,
                Category = transactionCategory,
                PhoneNumber = user.Phone, 
                BankDetails = transferRequest.BankDetails.Select(b => new TransferBank
                {
                    TransferredAmount = b.TransferredAmount,
                    BankName = b.BankName,
                    AccountName = b.AccountName,
                    AccountNumber = b.AccountNumber
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
                "your transfer in the dashboard\n",
                Data = true
            };
        }

        public async Task<Response<bool>> AccountPaymentAsync(AccountPaymentRequest accountPaymentRequest)
        {
            var user = await _userRepository.GetUserByEmailAsync(accountPaymentRequest.CustomerEmail);
            if (user == null)
                return new Response<bool> { Success = false, Message = "Email address is null", Data = false };

            var customer = await _customerRepository.GetCustomerByIdAsync(user.Id) ?? await _customerRepository.GetCustomerByEmailAsync(user.Email);
            if (customer == null)
                return new Response<bool> { Success = false, Message = "Customer details not found\nYou need to do atleast one transfer", Data = false };

            if (customer.Balance <= 0)
                return new Response<bool> { Success = false, Message = "You account balance is 0", Data = false };

            if (customer.Balance < accountPaymentRequest.NairaAmount)
                return new Response<bool> { Success = false, Message = "Insufficient account balance", Data = false };

            await _userRepository.SubtractTransferAmountAsync(accountPaymentRequest.CustomerEmail, accountPaymentRequest.NairaAmount);

            var data = new AccountPayment
            {
                NairaAmount = accountPaymentRequest.NairaAmount,
                DollarAmount = accountPaymentRequest.DollarAmount,
                Description = accountPaymentRequest.Description,
                CustomerAccount = customer.AccountNumber,
                Balance = customer.Balance,
                CustomerName = customer.Name,
                Channel = Channel.WalkIn
            };

            await _userRepository.AccountPaymentAsync(data);

            return new Response<bool>
            {
                Success = true,
                Message = "Your request has been successfully submitted for admin to verified. You will be notified once confirmed" +
                ".\n\nYou can confirmed the status of your transfer in the dashboard\n",
                Data = true
            };
        }

        public async Task<Response<bool>> ThirdPartyPaymentAsync(ThirdPartyPaymentRequest thirdPartyPaymentRequest)
        {
            if (string.IsNullOrEmpty(thirdPartyPaymentRequest.CustomerEmail))
                return new Response<bool> { Success = false, Message = "Email not passed", Data = false };

            var user = await _userRepository.GetUserByEmailAsync(thirdPartyPaymentRequest.CustomerEmail);
            if (user == null)
                return new Response<bool> { Success = false, Message = "Email address is null", Data = false };

            var customer = await _customerRepository.GetCustomerByIdAsync(user.Id) ?? await _customerRepository.GetCustomerByEmailAsync(user.Email);
            if (customer == null)
                return new Response<bool> { Success = false, Message = "Customer details not found\nYou need to do atleast one transfer", Data = false };

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
                Channel = Channel.WalkIn,
                Status = "Pending"
            };

            await _userRepository.ThirdPartyPaymentAsync(data);

            return new Response<bool>
            {
                Success = true,
                Message = " Your transfer details has been successful submitted\nYou’ll be notified once the transfer has been made.",
                Data = true
            };
        }

        private async Task<string> GenerateTransactionReference()
        {
            int refNumber;

            do
                refNumber = Random.Shared.Next(100000, 999999);
            while (await _userRepository.IsTransferRefrenceExistsAsync("ICNL" + refNumber.ToString()));
            return $"ICNL{refNumber}";
        }

        public async Task<Response<bool>> GetTransferStatus(string email)
        {
            var history = await _userRepository.GetTransactionHistory(email);

            if (!history.Any())
                return new Response<bool> { Success = false, Message = "User details not found", Data = false };
            
            bool hasPendingTransfer = history.Any(item => item.Status.ToLower().Contains("pending"));
            if (hasPendingTransfer)
            {
                return new Response<bool>
                {
                    Success = true,
                    Message = "Once your transfer is confirmed, you will be redirected to view and download transaction(s) related documents.",
                    Data = true
                };
            }

            return new Response<bool>
            {
                Success = true,
                Message = "You don’t have any pending transfer that required attention.\nCheck transaction history to view all your confirmed transfer receipts",
                Data = true
            };
        }

        public async Task<Response<List<TransactionHistoryResponse>>> GetTransactionHistory(string email)
        {
            var history = await _userRepository.GetTransactionHistory(email);

            var transactionHistory = history.Select(h => new TransactionHistoryResponse
            {
                Description = h.Description,
                TotalAmount = h.BankDetails.Sum(b => b.TransferredAmount).ToString("F2"),
                TransactionDate = h.TransactionDate.ToString(),
                Category = h.Category.ToString(),
                AccountDetails = h.BankDetails.Select(b => new AccountDetails
                {
                    AccountName = b.AccountName,
                    AccountNumber = b.AccountNumber,
                    Amount = b.TransferredAmount.ToString("F2"),
                    BankName = b.BankName
                }).ToList()
            }).ToList();

            return new Response<List<TransactionHistoryResponse>>
            {
                Success = true,
                Message = "Success",
                Data = transactionHistory
            };
        }

        public Task<Response<string>> GetRemitStatus(string email)
        {
            throw new NotImplementedException();
        }
    }
}

