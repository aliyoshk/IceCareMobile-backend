using System;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Core.Entities.Users;

namespace IceCareNigLtd.Infrastructure.Interfaces.Users
{
	public interface IUserRepository
	{
        Task<Registration> GetUserByIdAsync(int userId);
        Task<Registration> GetUserByEmailAsync(string email);
        Task<List<Registration>> GetUsersByStatusAsync(string status);
        Task AddUserAsync(Registration user);

        Task UpdateUserAsync(Registration user);
        Task MoveToApprovedAsync(Registration user);
        Task MoveToRejectedAsync(Registration user);
        Task DeleteUserAsync(int userId);

        // New method for checking account number existence
        Task<bool> IsAccountNumberExistsAsync(string accountNumber);
        Task<bool> IsPhoneNumberExistsAsync(string phoneNumber);
    }
}

