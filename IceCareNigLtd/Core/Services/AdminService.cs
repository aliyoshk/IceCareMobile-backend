using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Users;
using IceCareNigLtd.Core.Entities;
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

        public AdminService(IAdminRepository adminRepository, ITokenService tokenService, IConfiguration configuration,
            IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _adminRepository = adminRepository;
            _tokenService = tokenService;
            _configuration = configuration;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
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

        public async Task<Response<string>> ChangeUserStatusAsync(ChangeUserStatusRequest request, string adminName = null)
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
                    user.Reason = request.RejectUserDto.RejectionReason;
                    await _userRepository.MoveToApprovedAsync(user);
                    break;

                case ReviewAction.Reject:
                    user.Status = "Rejected";
                    if (request.RejectUserDto != null)
                    {
                        user.Reason = request.RejectUserDto.RejectionReason;
                    }
                    user.ReviewedBy = adminName;
                    await _userRepository.MoveToRejectedAsync(user);
                    break;

                default:
                    return new Response<string> { Success = false, Message = "Invalid action." };
            }

            return new Response<string> { Success = true, Message = $"User {request.Action} successfully." };
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
    }
}


