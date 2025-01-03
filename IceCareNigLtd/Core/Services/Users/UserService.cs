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

        public async Task<Response<string>> RegisterUserAsync(RegistrationDto registrationDto)
        {
            var existingUser = await _userRepository.GetRegisteredUserByEmail(registrationDto.Email);
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
            var newUser = new User
            {
                FullName = registrationDto.FullName,
                Email = registrationDto.Email,
                Phone = registrationDto.Phone,
                Password = hashedPassword,
                Date = DateTime.UtcNow,
                Status = "Pending",
                BalanceDollar = 0,
                BalanceNaira = 0,
                AccountNumber = "",
                Reviewer = ""
            };

            await _userRepository.RegisterUser(newUser);

            return new Response<string>
            {
                Success = true,
                Message = "Success",
                Data = "Thank you for registering with us!\nWe have received your details and it's currently under review by our admin team.\n\n" +
                "Please check back later to access your account and start using our services. We appreciate your patience and look forward to welcoming you on board!",
            };
        }

        public async Task<Response<LoginResponse>> LoginUserAsync(LoginDto loginDto)
        {
            if (string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
                return new Response<LoginResponse> { Success = false, Message = "Login details cannot be empty" };

            var user = await _userRepository.GetRegisteredUserByEmail(loginDto.Email);

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

            // Get current Dollar Rate
            var dollarRate = await _settingsRepository.GetDollarRateAsync();
            var companyPhone = await _settingsRepository.GetCompanyPhoneNumbersAsync();
            var companyAccounts = await _settingsRepository.GetCompanyAccountsAsync();
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
                NairaBalance = user.BalanceNaira.ToString("F2"),
                DollarBalance = user.BalanceDollar.ToString("F2"),
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
            var user = await _userRepository.GetRegisteredUserByEmail(resetPasswordRequest.Email);
            if (user == null)
                return new Response<bool> { Success = false, Message = "Email doesn't exist", Data = false };
            if (user.Phone != resetPasswordRequest.Phone)
                return new Response<bool> { Success = false, Message = "The phone number does not match our records.", Data = false };
            
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
            var dollarRate = await _settingsRepository.GetDollarRateAsync();
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

            var user = await _userRepository.GetRegisteredUserByEmail(transferRequest.CustomerEmail);
            if (user == null)
                return new Response<bool> { Success = false, Message = "User is null", Data = false };
            if (string.IsNullOrEmpty(user.Email))
                return new Response<bool> { Success = false, Message = "Email address is null", Data = false };
            if (user.Status.ToLower().Equals("pending"))
                return new Response<bool> { Success = false, Message = "Account is not approved yet, try again later", Data = false };
            if (string.IsNullOrEmpty(user.AccountNumber))
                return new Response<bool> { Success = false, Message = "Account number not yet generated, please try again later", Data = false };

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
                Currency = PaymentCurrency.Naira,
                CustomerName = user.FullName,
                Email = user.Email,
                DollarRate = dollarRate,
                TransferReference = transactionReference,
                Status = "Pending",
                Approver = user.Reviewer,
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

            var expectedDollar = transferRequest.BankDetails.Sum(a => a.TransferredAmount) / dollarRate;
            if (transferRequest.DollarAmount > expectedDollar)
            {
                var balance = (transferRequest.DollarAmount - expectedDollar) * dollarRate;
                await _userRepository.SubtractUserNairaBalance(user.Email, balance);
            }
            else if (expectedDollar > transferRequest.DollarAmount)
            {
                var balance = (expectedDollar - transferRequest.DollarAmount) * dollarRate;
                await _userRepository.AddUserNairaBalance(user.Email, balance);
            }


            return new Response<bool>
            {
                Success = true,
                Message = "You transfer details has been successfully submitted for admin to verify. " +
                "You will be notified once the transfer is confirmed.\n\nYou can confirmed the status of " +
                "your transfer in the dashboard\n",
                Data = true
            };
        }

        public async Task<Response<bool>> AccountPaymentAsync(AccountPaymentRequest accountPaymentRequest)
        {
            if (accountPaymentRequest.Amount <= 0)
                return new Response<bool> { Success = false, Message = "Amount cannot be less than or equal to Zero", Data = false };

            var user = await _userRepository.GetRegisteredUserByEmail(accountPaymentRequest.CustomerEmail);
            var dollarRate = await _settingsRepository.GetDollarRateAsync();

            if (user == null)
                return new Response<bool> { Success = false, Message = "User not found", Data = false };
            if (accountPaymentRequest.Amount > user.BalanceNaira)
                return new Response<bool> { Success = false, Message = "Amount is greather than account balance", Data = false };
            if (string.IsNullOrEmpty(accountPaymentRequest.CustomerEmail))
                return new Response<bool> { Success = false, Message = "Email is null", Data = false };
            if (accountPaymentRequest.CustomerEmail != user.Email)
                return new Response<bool> { Success = false, Message = "Request email doesn't match user record", Data = false };
            if (user.Status.ToLower().Equals("pending"))
                return new Response<bool> { Success = false, Message = "Account is not approved yet, try again later", Data = false };
            if (string.IsNullOrEmpty(user.AccountNumber))
                return new Response<bool> { Success = false, Message = "Account number not yet generated, please try again later", Data = false };
            if (user.BalanceNaira <= 0)
                return new Response<bool> { Success = false, Message = "Your account balance is less than or equal to Zero", Data = false };

            var transactionReference = await GenerateTransactionReference();
            var data = new AccountPayment
            {
                Description = accountPaymentRequest.Description,
                CustomerAccount = user.AccountNumber,
                Amount = accountPaymentRequest.Amount,
                CustomerName = user.FullName,
                Channel = Channel.WalkIn,
                DollarRate = dollarRate,
                DollarAmount = accountPaymentRequest.Amount / dollarRate,
                Currency = PaymentCurrency.Naira,
                Category = Category.AccountBalancePayment,
                ReferenceNo = transactionReference,
                Date = DateTime.UtcNow,
                Email = user.Email,
                Phone = user.Phone,
                Reviewer = "",
                Status = "Pending"
            };

            await _userRepository.AccountPaymentAsync(data);
            await _userRepository.SubtractUserNairaBalance(accountPaymentRequest.CustomerEmail, accountPaymentRequest.Amount);

            return new Response<bool>
            {
                Success = true,
                Message = "Your request has been successfully submitted for admin to verify and your balance will be updated once confirmed" +
                ".\n\nYou can check the status of your transfer in the dashboard",
                Data = true
            };
        }

        public async Task<Response<bool>> ThirdPartyPaymentAsync(ThirdPartyPaymentRequest thirdPartyPaymentRequest)
        {
            if (string.IsNullOrEmpty(thirdPartyPaymentRequest.CustomerEmail))
                return new Response<bool> { Success = false, Message = "Email not passed", Data = false };
            if (thirdPartyPaymentRequest.Amount <= 0)
                return new Response<bool> { Success = false, Message = "Amount cannot be less than or equal to Zero", Data = false };

            var user = await _userRepository.GetRegisteredUserByEmail(thirdPartyPaymentRequest.CustomerEmail);
            if (user == null)
                return new Response<bool> { Success = false, Message = "User not found", Data = false };
            if (thirdPartyPaymentRequest.Amount > user.BalanceNaira)
                return new Response<bool> { Success = false, Message = "Amount is greather than account balance", Data = false };
            if (thirdPartyPaymentRequest.CustomerEmail != user.Email)
                return new Response<bool> { Success = false, Message = "Request email doesn't match user record", Data = false };
            if (user.Status.ToLower().Equals("pending"))
                return new Response<bool> { Success = false, Message = "Account is not approved yet, try again later", Data = false };
            if (string.IsNullOrEmpty(user.AccountNumber))
                return new Response<bool> { Success = false, Message = "Account number not yet generated, please try again later", Data = false };

            var transactionReference = await GenerateTransactionReference();
            var data = new ThirdPartyPayment
            {
                Date = DateTime.UtcNow,
                Amount = thirdPartyPaymentRequest.Amount,
                AccountName = thirdPartyPaymentRequest.AccountName,
                AccountNumber = thirdPartyPaymentRequest.AccountNumber,
                BankName = thirdPartyPaymentRequest.BankName,
                Description = thirdPartyPaymentRequest.Description,
                CustomerAccount = user.AccountNumber,
                CustomerName = user.FullName,
                Channel = Channel.WalkIn,
                Email = user.Email,
                Category = Category.ThirdPartyPayment,
                ReferenceNo = transactionReference,
                Status = "Pending",
                Approver = "",
            };

            await _userRepository.ThirdPartyPaymentAsync(data);
            await _userRepository.SubtractUserNairaBalance(thirdPartyPaymentRequest.CustomerEmail, thirdPartyPaymentRequest.Amount);

            return new Response<bool>
            {
                Success = true,
                Message = " Your transfer details has been successful submitted\nYou can check the request status from your dashboard.",
                Data = true
            };
        }

        public async Task<Response<bool>> TopUpAccountAsync(AccoutTopUpRequest accoutTopUpRequest)
        {
            if (string.IsNullOrEmpty(accoutTopUpRequest.Email))
                return new Response<bool> { Success = false, Message = "Email not passed", Data = false };
            if (!accoutTopUpRequest.BankDetails.Any())
                return new Response<bool> { Success = false, Message = "Transfer details cannot be empty", Data = false };

            var user = await _userRepository.GetRegisteredUserByEmail(accoutTopUpRequest.Email);
            if (user == null)
                return new Response<bool> { Success = false, Message = "User not found", Data = false };
            if (user.Status.ToLower().Equals("pending"))
                return new Response<bool> { Success = false, Message = "Account is not approved yet, try again later", Data = false };
            if (string.IsNullOrEmpty(user.AccountNumber))
                return new Response<bool> { Success = false, Message = "Account number not yet generated, please try again later", Data = false };
            if (user.Email != accoutTopUpRequest.Email)
                return new Response<bool> { Success = false, Message = "Email doesn't match record", Data = false };
            if (user.Phone != accoutTopUpRequest.Phone)
                return new Response<bool> { Success = false, Message = "Phone number doesn't match record", Data = false };


            var errorMessages = new List<string>();
            foreach (var bank in accoutTopUpRequest.BankDetails)
            {
                if (string.IsNullOrEmpty(bank.BankName))
                    errorMessages.Add($"Select Bank, field cannot be empty");

                if (bank.TransferredAmount <= 0)
                    errorMessages.Add($"The amount transferred for {bank.BankName} should be greather than 0");

                if (string.IsNullOrEmpty(bank.AccountNumber))
                    errorMessages.Add($"The account number for {bank.BankName} is missing");
            }

            if (errorMessages.Any())
                return new Response<bool> { Success = false, Message = string.Join("; ", errorMessages) };

            var currency = PaymentCurrency.Dollar;
            decimal amount = 0;
            if (accoutTopUpRequest.Currency.ToLower().Contains("naira"))
            {
                currency = PaymentCurrency.Naira;
            }
            var transactionReference = await GenerateTransactionReference();

            var data = new AccountTopUp
            {
                Currency = currency,
                Status = "Pending",
                TransactionDate = DateTime.UtcNow,
                Description = accoutTopUpRequest.Description,
                Reference = transactionReference,
                Category = Category.AccountTopUp,
                Email = accoutTopUpRequest.Email,
                Name = user.FullName,
                AccountNo = user.AccountNumber,
                Phone = user.Phone,
                Approver = "",
                TransferDetails = accoutTopUpRequest.BankDetails.Select(b => new TransferDetail
                {
                    TransferredAmount = b.TransferredAmount,
                    BankName = b.BankName,
                    AccountName = b.AccountName,
                    AccountNumber = b.AccountNumber,
                }).ToList(),
                TransferEvidence = accoutTopUpRequest.TransferEvidence.Select(e => new TopUpEvidence
                {
                    Receipts = e.Receipts
                }).ToList()
            };
            await _userRepository.TopUpAccountAsync(data);

            return new Response<bool>
            {
                Success = true,
                Message = "Your account top up request has been submitted successfully.\n" +
                "The value will reflect on dashboard once admin confirmed the remittance",
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

