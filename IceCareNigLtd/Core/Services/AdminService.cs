using System.Security.Cryptography.Xml;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Request;
using IceCareNigLtd.Api.Models.Response;
using IceCareNigLtd.Api.Models.Users;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Core.Entities.Users;
using IceCareNigLtd.Core.Interfaces;
using IceCareNigLtd.Infrastructure.Interfaces;
using IceCareNigLtd.Infrastructure.Interfaces.Users;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Core.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        private readonly ISupplierRepository _supplierRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBankRepository _bankRepository;

        public AdminService(IAdminRepository adminRepository, ITokenService tokenService, IConfiguration configuration,
            IUserRepository userRepository, IPasswordHasher passwordHasher, ISupplierRepository supplierRepository,
            ICustomerRepository customerRepository, IBankRepository bankRepository)
        {
            _adminRepository = adminRepository;
            _tokenService = tokenService;
            _configuration = configuration;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _supplierRepository = supplierRepository;
            _customerRepository = customerRepository;
            _bankRepository = bankRepository;
        }

        public async Task<Response<AdminDto>> AddAdminAsync(AdminDto adminDto)
        {
            try
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
                    //Password = _passwordHasher.HashPassword(adminDto.Password),
                    Password = adminDto.Password,
                    Role = "Normal",
                    Date = DateTime.UtcNow
                };

                await _adminRepository.AddAdminAsync(admin);

                return new Response<AdminDto> { Success = true, Message = "Admin added successfully", Data = adminDto };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding admin: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task<Response<List<AdminDto>>> GetAdminsAsync()
        {
            var admins = await _adminRepository.GetAdminsAsync();
            var adminDtos = admins.Select(a => new AdminDto
            {
                Id = a.Id,
                Name = a.Name,
                Email = a.Email,
                Role = a.Role,
                Date = a.Date,
                Password = a.Password
            }).ToList();

            return new Response<List<AdminDto>> { Success = true, Message = "Admins retrieved successfully", Data = adminDtos };
        }

        public async Task<Response<object>> DeleteAdminAsync(int adminId)
        {
            await _adminRepository.DeleteAdminAsync(adminId);
            return new Response<object> { Success = true, Message = "Admin deleted successfully" };
        }


        public async Task<Response<AdminResponseDto>> LoginAsync(AdminLoginDto adminLoginDto)
        {
            if (adminLoginDto == null)
            {
                return new Response<AdminResponseDto> { Success = false, Message = "Login data cannot be null" };
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
                var token = _tokenService.GenerateToken(admin.Email, admin.Name);
                var response = new AdminResponseDto()
                {
                    AdminName = admin.Name,
                    ShowAdminPanel = admin.Role.ToLower() != "normal" ? true : false,
                    Role = admin.Role,
                    Token = token
                };

                return new Response<AdminResponseDto> { Success = true, Message = "Login successful", Data = response };
            }

            return new Response<AdminResponseDto> { Success = false, Message = "Invalid credentials" };
        }



        //MOBILE PART INTEGRATION
        public async Task<Response<List<UserDto>>> GetUsersByStatusAsync(string status)
        {
            var users = await _userRepository.GetRegisteredUserByStatus(status);
            var userDtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Status = user.Status,
                Phone = user.Phone,
                AccountNumber = user.AccountNumber,
                ReviewedBy = user.Reviewer
            }).ToList();

            return new Response<List<UserDto>> { Success = true, Data = userDtos };
        }

        public async Task<Response<string>> ChangeUserStatusAsync(ChangeUserStatusRequest request, string adminName = "")
        {
            var user = await _userRepository.GetRegisteredUserById(request.UserId);
            if (user == null)
            {
                return new Response<string> { Success = false, Message = "User not found." };
            }

            switch (request.Action)
            {
                case ReviewAction.Approve:
                    var accountNumber = await GenerateUniqueAccountNumberAsync();
                    user.Status = "Approved";
                    user.AccountNumber = accountNumber;
                    user.Reviewer = adminName;
                    await _userRepository.MoveToApprovedAsync(user);
                    break;

                case ReviewAction.Reject:
                    user.Status = "Rejected";
                    user.Reviewer = adminName;
                    await _userRepository.MoveToRejectedAsync(user);
                    break;

                default:
                    return new Response<string> { Success = false, Message = "Invalid action." };
            }

            return new Response<string> { Success = true, Message = "Success", Data = $"User {request.Action} successfully." };
        }

        public async Task<Response<object>> DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetRegisteredUserById(userId);
            if (user.BalanceNaira > 0 && user.BalanceDollar > 0)
                return new Response<object> { Success = false, Message = "User has an existing balance in the system" };

            await _userRepository.DeleteUserAsync(userId);
            return new Response<object> { Success = true, Message = "User deleted successfully" };
        }

        public async Task<Response<List<UserDto>>> GetApprovedUsersAsync()
        {
            var approvedUsers = await _userRepository.GetRegisteredUserByStatus("Approved");
            var approvedUserDtos = approvedUsers.Select(user => new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Status = user.Status,
                AccountNumber = user.AccountNumber,
                Phone = user.Phone,
                ReviewedBy = user.Reviewer
            }).ToList();

            return new Response<List<UserDto>> { Success = true, Data = approvedUserDtos };
        }

        public async Task<Response<List<UserDto>>> GetRejectedUsersAsync()
        {
            var rejectedUsers = await _userRepository.GetRegisteredUserByStatus("Rejected");
            var rejectedUserDtos = rejectedUsers.Select(user => new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Status = user.Status,
                ReviewedBy = user.Reviewer,
                AccountNumber = user.AccountNumber,
                Phone = user.Phone
            }).ToList();

            return new Response<List<UserDto>> { Success = true, Data = rejectedUserDtos };
        }

        private async Task<string> GenerateUniqueAccountNumberAsync()
        {
            var random = new Random();
            long accountNumber;
            do
            {
                accountNumber = random.NextInt64(1000000000L, 10000000000L);
            }
            while (await _userRepository.IsAccountNumberExistsAsync(accountNumber.ToString())); // Ensure it’s unique
            return accountNumber.ToString();
        }

        public async Task<Response<List<TransferResponse>>> GetPendingTransferAsync()
        {
            var pendingTransfers = await _userRepository.GetTransferByStatusAsync("Pending");
            var pendingTransfersDtos = pendingTransfers.Select(users => new TransferResponse
            {
                Id = users.Id,
                CustomerName = users.CustomerName,
                TransferReference = users.TransferReference,
                Status = users.Status,
                ApproverName = users.Approver,
                Category = users.Category.ToString(),
                Description = users.Description,
                DollarRate = users.DollarRate,
                DollarAmount = users.DollarAmount,
                TransactionDate = users.TransactionDate,
                CustomerEmail = users.Email,
                PhoneNumber = users.PhoneNumber,
                BankDetails = users.BankDetails.Select(b => new TransferDetails
                {
                    BankName = b.BankName.ToString(),
                    AmountTransferred = b.TransferredAmount,
                    AccountName = b.AccountName,
                    AccountNumber = b.AccountNumber
                }).ToList(),
                TransferEvidence = users.TransferEvidence.Select(e => new TransferEvidence
                {
                    Receipts = e.Receipts
                }).ToList()
            }).ToList();

            return new Response<List<TransferResponse>>
            {
                Success = true,
                Message = "Success",
                Data = pendingTransfersDtos
            };
        }

        public async Task<Response<List<TransferResponse>>> GetApprovedTransferAsync()
        {
            var confirmedTransfers = await _userRepository.GetTransferByStatusAsync("Confirmed");
            var confirmedTransfersDtos = confirmedTransfers.Select(users => new TransferResponse
            {
                Id = users.Id,
                CustomerName = users.CustomerName,
                Status = users.Status,
                ApproverName = users.Approver,
                Category = users.Category.ToString(),
                Description = users.Description,
                DollarRate = users.DollarRate,
                DollarAmount = users.DollarAmount,
                TransactionDate = users.TransactionDate,
                CustomerEmail = users.Email,
                PhoneNumber = users.PhoneNumber,
                TransferReference = users.TransferReference,
                BankDetails = users.BankDetails.Select(b => new TransferDetails
                {
                    BankName = b.BankName.ToString(),
                    AmountTransferred = b.TransferredAmount,
                    AccountName = b.BankName,
                    AccountNumber = b.AccountNumber
                }).ToList(),
                TransferEvidence = users.TransferEvidence.Select(e => new TransferEvidence
                {
                    Receipts = e.Receipts
                }).ToList()
            }).ToList();

            return new Response<List<TransferResponse>>
            {
                Success = true,
                Message = "Success",
                Data = confirmedTransfersDtos
            };
        }

        public async Task<Response<string>> ConfirmTransferAsync(ConfirmationRequest request, string adminName = null)
        {
            var user = await _userRepository.GetTransferByIdAsync(request.Id);
            if (user == null)
                return new Response<string> { Success = false, Message = "User not found.", Data = "User not found." };
            if (user.Email != request.Email)
                return new Response<string> { Success = false, Message = "Email not found.", Data = "Email not found." };
            if (user.Id != request.Id)
                return new Response<string> { Success = false, Message = "Id not found", Data = "Email not found." };
            var userRecord = await _userRepository.GetRegisteredUserByEmail(user.Email);

            if (request.Confirmed)
            {
                user.Status = "Confirmed";
                user.Approver = adminName;
                await _userRepository.AddUserNairaBalance(user.Email, user.BankDetails.Sum(a => a.TransferredAmount));
                await _userRepository.ApproveTransferAsync(user);
            }
            else
                return new Response<string> { Success = false, Message = "Error.", Data = "Invalid Action"};

            var userDetails = await _userRepository.GetRegisteredUserByEmail(user.Email);
            decimal amount = user.BankDetails.Sum(a => a.TransferredAmount);
            var customer = new Customer
            {
                Name = user.CustomerName,
                Date = DateTime.UtcNow,
                ModeOfPayment = ModeOfPayment.Transfer,
                DollarRate = user.DollarRate,
                DollarAmount = user.DollarAmount,
                TotalNairaAmount = amount,
                Balance = userDetails.BalanceNaira,
                PhoneNumber = userRecord.Phone ?? "",
                PaymentCurrency = PaymentCurrency.Naira,
                Channel = Channel.Mobile,
                AccountNumber = user.CustomerAccount,
                Deposit = userDetails.BalanceNaira,
                PaymentEvidence = user.TransferEvidence.Select(e => new CustomerPaymentReceipt
                {
                    Reciept = e.Receipts
                }).ToList(),
                Banks = user.BankDetails.Select(b => new CustomerBankInfo
                {
                    BankName = b.BankName.ToString(),
                    AmountTransferred = b.TransferredAmount,
                }).ToList()
            };

            await _supplierRepository.SubtractDollarAmountAsync(customer.DollarAmount);
            await _customerRepository.AddCustomerAsync(customer);

            foreach (var bankInfo in customer.Banks)
            {
                var bank = new Bank
                {
                    BankName = bankInfo.BankName.ToString(),
                    Date = DateTime.UtcNow,
                    EntityName = user.CustomerName,
                    PersonType = PersonType.Customer,
                    ExpenseType = CreditType.Credit,
                    Amount = bankInfo.AmountTransferred,
                };

                await _bankRepository.AddBankAsync(bank);
            }
            // we aren't deleting record at the moment
            //await _userRepository.DeleteCustomerTransferRecordAsync(user.Id);

            return new Response<string>
            {
                Success = true,
                Message = "Success",
                Data = $"User transfer status has been confirmed by {adminName}."
            };
        }

        public async Task<Response<List<TransferResponse>>> GetUsersByTransferStatusAsync(string status)
        {
            var users = await _userRepository.GetTransferByStatusAsync(status);

            var details = users.Select(user => new TransferResponse
            {
                Id = user.Id,
                CustomerName = user.CustomerName,
                TransferReference = user.TransferReference,
                ApproverName = user.Approver,
                DollarAmount = user.DollarAmount,
                Category = user.Category.ToString(),
                CustomerEmail = user.Email,
                Description = user.Description,
                Status = user.Status,
                DollarRate = user.DollarRate,
                TransactionDate = user.TransactionDate,
                PhoneNumber = user.PhoneNumber,
                TransferEvidence = user.TransferEvidence?.Select(e => new TransferEvidence
                {
                    Receipts = e.Receipts
                }).ToList() ?? new List<TransferEvidence>(),
                BankDetails = user.BankDetails?.Select(b => new TransferDetails
                {
                    BankName = b.BankName.ToString(),
                    AmountTransferred = b.TransferredAmount,
                    AccountName = b.AccountName,
                    AccountNumber = b.AccountNumber
                }).ToList() ?? new List<TransferDetails>(),
            }).ToList();

            return new Response<List<TransferResponse>> { Success = true, Data = details };
        }

        public async Task<Response<object>> DeleteTransferRecordAsync(int id)
        {
            await _userRepository.DeleteTransferRecordAsync(id);
            return new Response<object> { Success = true, Message = "User deleted successfully" };
        }

        public async Task<Response<List<TransferResponse>>> GetAccountPaymentAsync(string status)
        {
            var user = await _userRepository.GetAccountPayments(status);
            var data = user.Select(users => new TransferResponse
            {
                Id = users.Id,
                CustomerName = users.CustomerName,
                Status = users.Status,
                ApproverName = users.Reviewer,
                Category = users.Category.ToString(),
                Description = users.Description,
                DollarRate = users.DollarRate,
                DollarAmount = users.DollarAmount,
                TransactionDate = users.Date,
                CustomerEmail = users.Email,
                PhoneNumber = users.Phone,
                TransferReference = users.ReferenceNo,
                BankDetails = null,
                TransferEvidence = null
            }).ToList();

            return new Response<List<TransferResponse>>
            {
                Success = true,
                Message = "Success",
                Data = data
            };
        }

        public async Task<Response<string>> ConfirmAccountPaymentAsync(int id, string adminName = null)
        {
            var user = await _userRepository.GetAccountPaymentById(id);
            if (user == null)
                return new Response<string> { Success = false, Message = "Record not found.", Data = "Value not found." };

            user.Status = "Confirmed";
            user.Reviewer = adminName;

            await _userRepository.ConfirmAccountPayment(user);

            return new Response<string>
            {
                Success = true,
                Message = "Success",
                Data = "Account payment has been confirmed"
            };
        }

        public async Task<Response<object>> DeleteAccountPaymentAsync(int id)
        {
            var user = await _userRepository.GetUserAccountTopUpAsync(id);
            if (user.Status == "pending")
                await _userRepository.AddUserNairaBalance(user.Email, user.TransferDetails.Sum(a => a.TransferredAmount));

            await _userRepository.DeleteAccountPaymentRecordAsync(id);
            return new Response<object> { Success = true, Message = "User deleted successfully" };
        }

        public async Task<Response<List<ThirdPartyPaymentResponse>>> GetThirdPartyTransfer(string status)
        {
            var transfer = await _userRepository.GetThirdPartyTransfers(status);
            var transferDtos = transfer.Select(t => new ThirdPartyPaymentResponse
            {
                Description = t.Description,
                AccountName = t.AccountName,
                AccountNumber = t.AccountNumber,
                Amount = t.Amount,
                BankName = t.BankName,
                Status = t.Status,
                CustomerAccount = t.CustomerAccount,
                Channel = t.Channel.ToString(),
                CustomerName = t.CustomerName
            }).ToList();

            return new Response<List<ThirdPartyPaymentResponse>>
            {
                Success = true,
                Message = "Success",
                Data = transferDtos
            };
        }

        public async Task<Response<string>> ThirdPartyTransferCompleted(int id, string adminName = null)
        {
            var transfer = await _userRepository.GetThirdPartyPaymentById(id);
            if (transfer == null)
                return new Response<string> { Success = false, Message = "Value not found.", Data = "Value not found." };

            //await _userRepository.SubtractUserNairaBalance(transfer.Email, transfer.Amount);

            transfer.Status = "Confirmed";
            transfer.Approver = adminName;

            await _userRepository.ThirdPartyTransferCompleted(transfer);

            return new Response<string>
            {
                Success = true,
                Message = "Success",
                Data = "User transfer has been confirmed"
            };
        }

        public async Task<Response<object>> DeleteThirdPartyTransferAsync(int id)
        {
            var user = await _userRepository.GetTransferByIdAsync(id);
            if (user.Status.ToLower() == "pending")
                await _userRepository.AddUserNairaBalance(user.Email, user.BankDetails.Sum(a => a.TransferredAmount));

            await _userRepository.DeleteThirdPartyTransferRecordAsync(id);
            return new Response<object> { Success = true, Message = "User deleted successfully" };
        }

        public async Task<Response<List<AccountTopUpResponse>>> GetAccountTopUpsAsync(string status)
        {
            var response = await _userRepository.GetAccountTopUpsAsync(status);
            var accountTopUps = response.Select(t => new AccountTopUpResponse
            {
                Id = t.Id,
                Amount = t.TransferDetails.Sum(a => a.TransferredAmount),
                CustomerName = t.Name,
                Currency = t.Currency.ToString(),
                Email = t.Email,
                Phone = t.Phone,
                Description = t.Description,
                Status = t.Status,
                BankDetails = t.TransferDetails.Select(b => new BankDetails
                {
                    AccountName = b.AccountName,
                    AccountNumber = b.AccountNumber,
                    TransferredAmount = b.TransferredAmount,
                    BankName = b.BankName
                }).ToList(),
                TransferEvidence = t.TransferEvidence?.Select(e => new TransferEvidence
                {
                    Receipts = e.Receipts
                }).ToList() ?? new List<TransferEvidence>(),
            }).ToList();

            return new Response<List<AccountTopUpResponse>>
            {
                Success = true,
                Message = "Success",
                Data = accountTopUps
            };
        }

        public async Task<Response<string>> ConfirmAccountTopUp(ConfirmationRequest request, string adminName = null)
        {
            var user = await _userRepository.GetUserAccountTopUpAsync(request.Id);
            if (user == null)
                return new Response<string> { Success = false, Message = "User not found.", Data = "User not found." };
            if (user.Status.ToLower() == "confirmed")
                return new Response<string> { Success = false, Message = "Top up already confirmed"};
            if (user.Email != request.Email)
                return new Response<string> { Success = false, Message = "Email not found.", Data = "Email not found." };
            if (user.Id != request.Id)
                return new Response<string> { Success = false, Message = "Id not found", Data = "Id not found." };

            user.Status = "Confirmed";
            user.Approver = adminName;
            await _userRepository.ConfirmAccountTopUp(user);

            await _userRepository.AddUserNairaBalance(user.Email, user.TransferDetails.Sum(a => a.TransferredAmount));

            return new Response<string>
            {
                Success = true,
                Message = "Success",
                Data = $"User top up status has been confirmed by {adminName}."
            };
        }

        public async Task<Response<object>> DeleteAccountTopUpAsync(int id)
        {
            var user = await _userRepository.GetUserAccountTopUpAsync(id);
            if (user.Status.ToLower() == "pending")
                await _userRepository.AddUserNairaBalance(user.Email, user.TransferDetails.Sum(a => a.TransferredAmount));

            await _userRepository.DeleteAccountTopUpRecordAsync(id);
            return new Response<object> { Success = true, Message = "User deleted successfully" };
        }
    }
}