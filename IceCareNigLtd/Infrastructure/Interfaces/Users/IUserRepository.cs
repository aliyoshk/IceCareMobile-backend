using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Request;
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
        Task<bool> IsAccountNumberExistsAsync(string accountNumber);
        Task<bool> IsPhoneNumberExistsAsync(string phoneNumber);
        Task ResetPasswordAsync(Registration user);

        Task FundTransferAsync(Transfer transfer);
        Task AccountPaymentAsync(AccountPayment accountPayment);
        Task ThirdPartyPaymentAsync(ThirdPartyPayment thirdPartyPayment);
    }
}


