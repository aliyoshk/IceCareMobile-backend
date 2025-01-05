using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Request;
using IceCareNigLtd.Api.Models.Response;
using IceCareNigLtd.Api.Models.Users;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Core.Entities.Users;

namespace IceCareNigLtd.Infrastructure.Interfaces.Users
{
	public interface IUserRepository
	{
        Task RegisterUser(User user);
        Task UpdateUserAsync(User user);
        Task MoveToApprovedAsync(User user);
        Task MoveToRejectedAsync(User user);
        Task DeleteUserAsync(int userId);

        Task<List<User>> GetRegisteredUsers();
        Task<User> GetRegisteredUserById(int id);
        Task<User> GetRegisteredUserByEmail(string email);
        Task<List<User>> GetRegisteredUserByStatus(string status);
        Task AddUserNairaBalance(string email, decimal amount);
        Task SubtractUserNairaBalance(string email, decimal amount);
        Task AddUserDollarBalance(string email, decimal amount);
        Task SubtractUserDollarBalance(string email, decimal amount);

        Task<bool> IsAccountNumberExistsAsync(string accountNumber);
        Task<bool> IsPhoneNumberExistsAsync(string phoneNumber);
        Task ResetPasswordAsync(User user);


        Task FundTransferAsync(Transfer transfer);
        Task<Transfer> GetTransferByIdAsync(int id);
        Task ApproveTransferAsync(Transfer user);
        Task<List<Transfer>> GetTransferByStatusAsync(string status);
        Task DeleteTransferRecordAsync(int id);
        Task<bool> IsTransferRefrenceExistsAsync(string transactionReference);

        Task AccountPaymentAsync(AccountPayment accountPayment);
        Task<List<AccountPayment>> GetAccountPayments(string status);
        Task<AccountPayment> GetAccountPaymentById(int id);
        Task ConfirmAccountPayment(AccountPayment accountPayment);
        Task DeleteAccountPaymentRecordAsync(int id);

        Task ThirdPartyPaymentAsync(ThirdPartyPayment thirdPartyPayment);
        Task<List<ThirdPartyPayment>> GetThirdPartyTransfers(string status);
        Task<ThirdPartyPayment> GetThirdPartyPaymentById(int id);
        Task ThirdPartyTransferCompleted(ThirdPartyPayment thirdPartyPayment);
        Task DeleteThirdPartyTransferRecordAsync(int id);

        Task TopUpAccountAsync(AccountTopUp accountTopUp);
        Task<List<AccountTopUp>> GetAccountTopUpsAsync(string status);
        Task<AccountTopUp> GetUserAccountTopUpAsync(int id);
        Task ConfirmAccountTopUp (AccountTopUp accountTopUp);
        Task DeleteAccountTopUpRecordAsync(int id);

        Task <List<Transfer>> GetTransferHistory(string email);
        Task<List<AccountPayment>> GetAccountPaymentHistory(string email);
        Task<List<ThirdPartyPayment>> GetThirdPartyHistory(string email);
        Task<List<AccountTopUp>> GetAccountTopUpHistory(string email);
        Task<Transfer> GetRemitStatus(string email);
    }
}


