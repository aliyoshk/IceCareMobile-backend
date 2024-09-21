using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Request;
using IceCareNigLtd.Api.Models.Response;
using IceCareNigLtd.Api.Models.Users;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Core.Interfaces;
using IceCareNigLtd.Infrastructure.Interfaces;
using IceCareNigLtd.Infrastructure.Interfaces.Users;
using IceCareNigLtd.Infrastructure.Repositories;
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
                Password = _passwordHasher.HashPassword(adminDto.Password),
                Role = "Normal",
                Date = DateTime.Now
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
                Password = a.Password
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
                var token = _tokenService.GenerateToken(admin.Email, admin.Name);
                return new Response<string> { Success = true, Message = "Login successful", Data = token };
            }

            return new Response<string> { Success = false, Message = "Invalid credentials" };
        }



        //MOBILE PART INTEGRATION
        public async Task<Response<List<UserDto>>> GetUsersByStatusAsync(string status)
        {
            var users = await _userRepository.GetUsersByStatusAsync(status);
            var userDtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Status = user.Status,
                Phone = user.Phone,
                AccountNumber = user.AccountNumber,
                Reason = user.Reason,
                ReviewedBy = user.ReviewedBy
            }).ToList();

            return new Response<List<UserDto>> { Success = true, Data = userDtos };
        }

        public async Task<Response<string>> ChangeUserStatusAsync(ChangeUserStatusRequest request, string adminName = "")
        {
            var user = await _userRepository.GetUserByIdAsync(request.UserId);
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
                    user.ReviewedBy = adminName;
                    user.Reason = "";
                    await _userRepository.MoveToApprovedAsync(user);
                    break;

                case ReviewAction.Reject:
                    user.Status = "Rejected";
                    user.ReviewedBy = adminName;
                    await _userRepository.MoveToRejectedAsync(user);
                    break;

                default:
                    return new Response<string> { Success = false, Message = "Invalid action." };
            }

            return new Response<string> { Success = true, Message = "Success", Data = $"User {request.Action} successfully." };
        }


        public async Task<Response<List<UserDto>>> GetApprovedUsersAsync()
        {
            var approvedUsers = await _userRepository.GetUsersByStatusAsync("Approved");
            var approvedUserDtos = approvedUsers.Select(user => new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Status = user.Status,
                AccountNumber = user.AccountNumber,
                Phone = user.Phone,
                Reason = user.Reason,
                ReviewedBy = user.ReviewedBy
            }).ToList();

            return new Response<List<UserDto>> { Success = true, Data = approvedUserDtos };
        }

        public async Task<Response<List<UserDto>>> GetRejectedUsersAsync()
        {
            var rejectedUsers = await _userRepository.GetUsersByStatusAsync("Rejected");
            var rejectedUserDtos = rejectedUsers.Select(user => new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Status = user.Status,
                Reason = user.Reason,
                ReviewedBy = user.ReviewedBy,
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
                Balance = users.Balance,
                TransferReference = users.TransferReference,
                Status = users.Status,
                ApproverName = users.Approver,
                Category = users.Category.ToString(),
                Description = users.Description,
                DollarRate = users.DollarRate,
                DollarAmount = users.DollarAmount,
                TransactionDate = users.TransactionDate,
                CustomerEmail = users.Email,
                BankDetails = users.BankDetails.Select(b => new BankInfoDto
                {
                    BankName = b.BankName.ToString(),
                    AmountTransferred = b.TransferredAmount
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
                Balance = users.Balance,
                TransferReference = users.TransferReference,
                BankDetails = users.BankDetails.Select(b => new BankInfoDto
                {
                    BankName = b.BankName.ToString(),
                    AmountTransferred = b.TransferredAmount
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
            var userRecord = await _userRepository.GetUserByEmailAsync(user.Email);

            if (request.Confirmed)
            {
                user.Status = "Confirmed";
                user.Approver = adminName;
                await _userRepository.ApproveTransferAsync(user);
            }
            else
                return new Response<string> { Success = false, Message = "Error.", Data = "Invalid Action"};


            decimal amount = user.BankDetails.Sum(a => a.TransferredAmount);
            var customer = new Customer
            {
                Name = user.CustomerName,
                Date = user.TransactionDate,
                ModeOfPayment = ModeOfPayment.Transfer,
                DollarRate = user.DollarRate,
                DollarAmount = user.DollarAmount,
                TotalNairaAmount = amount,
                Balance = user.Balance,
                PhoneNumber = userRecord.Phone ?? "",
                PaymentCurrency = PaymentCurrency.Naira,
                Channel = Channel.Mobile,
                AccountNumber = user.CustomerAccount,
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
                Balance = user.Balance,
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
                BankDetails = user.BankDetails?.Select(b => new BankInfoDto
                {
                    BankName = b.BankName.ToString(),
                    AmountTransferred = b.TransferredAmount
                }).ToList() ?? new List<BankInfoDto>(),
            }).ToList();

            return new Response<List<TransferResponse>> { Success = true, Data = details };
        }

        public async Task<Response<List<ThirdPartyPaymentResponse>>> GetThirdPartyTransfer()
        {
            var transfer = await _userRepository.GetThirdPartyTransfers();
            var transferDtos = transfer.Select(t => new ThirdPartyPaymentResponse
            {
                Description = t.Description,
                AccountName = t.AccountName,
                AccountNumber = t.AccountNumber,
                Amount = t.Amount,
                BankName = t.BankName,
                Status = t.Status,
                CustomerAccount = t.CustomerAccount,
                Balance = t.Balance,
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

        public async Task<Response<string>> ThirdPartyTransferCompleted(int id)
        {
            var transfer = await _userRepository.GetThirdPartyPaymentById(id);
            if (transfer == null)
                return new Response<string> { Success = false, Message = "Value not found.", Data = "Value not found." };

            transfer.Status = "Completed";
            await _userRepository.ThirdPartyTransferCompleted(transfer);

            return new Response<string>
            {
                Success = true,
                Message = "Success",
                Data = "User transfer has been confirmed"
            };
        }
    }
}