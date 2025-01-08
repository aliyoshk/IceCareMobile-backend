using System;
using System.Net.NetworkInformation;
using System.Security.Cryptography.Xml;
using IceCareNigLtd.Api.Models.Request;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Core.Entities.Users;
using IceCareNigLtd.Infrastructure.Data;
using IceCareNigLtd.Infrastructure.Interfaces.Users;
using Microsoft.EntityFrameworkCore;

namespace IceCareNigLtd.Infrastructure.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task RegisterUser(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task MoveToApprovedAsync(User user)
        {
            user.Status = "Approved";
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task MoveToRejectedAsync(User user)
        {
            user.Status = "Rejected";
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        
        public async Task<List<User>> GetRegisteredUsers() => await _context.Users.ToListAsync();

        public async Task<User> GetRegisteredUserById(int id) => await _context.Users.FindAsync(id);

        public async Task<User> GetRegisteredUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<List<User>> GetRegisteredUserByStatus(string status)
        {
            return await _context.Users
                .Where(u => u.Status == status)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddUserNairaBalance(string email, decimal amount)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if (amount <= 0)
                    return;

                user.BalanceNaira += amount;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SubtractUserNairaBalance(string email, decimal amount)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if (amount <= 0)
                    return;

                if (user.BalanceNaira >= amount)
                {
                    user.BalanceNaira -= amount;
                }
                else
                    return;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddUserDollarBalance(string email, decimal amount)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if (amount <= 0)
                    return;

                user.BalanceDollar += amount;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SubtractUserDollarBalance(string email, decimal amount)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if (amount <= 0)
                    return;

                if (user.BalanceDollar >= amount)
                {
                    user.BalanceDollar -= amount;
                }
                else
                    return;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsAccountNumberExistsAsync(string accountNumber)
        {
            return await _context.Users.AnyAsync(u => u.AccountNumber == accountNumber);
        }

        public async Task<bool> IsPhoneNumberExistsAsync(string phoneNumber)
        {
            return await _context.Users.AnyAsync(p => p.Phone == phoneNumber);
        }

        public async Task ResetPasswordAsync(User user)
        {
            var userDetail = await _context.Users.FindAsync(user.Id);
            if (userDetail != null)
            {
                userDetail.Password = user.Password;
                _context.Users.Update(userDetail);
                await _context.SaveChangesAsync();
            }
        }

        public async Task FundTransferAsync(Transfer transfer)
        {
            try
            {
                await _context.Transfers.AddAsync(transfer);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving transfer: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task ApproveTransferAsync(Transfer user)
        {
            _context.Transfers.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<Transfer> GetTransferByIdAsync(int id)
        {
            return await _context.Transfers
                .OrderByDescending(t => t.TransactionDate)
                .Include(t => t.BankDetails)
                .Include(e => e.TransferEvidence)
                .FirstAsync(t => t.Id == id);
        }

        public async Task<List<Transfer>> GetTransferByStatusAsync(string status)
        {
            return await _context.Transfers
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.TransactionDate)
                .Include(t => t.BankDetails)
                .Include(t => t.TransferEvidence)
                .ToListAsync();
        }

        public async Task DeleteTransferRecordAsync(int id)
        {
            var user = await _context.Transfers.FindAsync(id);
            if (user != null)
            {
                _context.Transfers.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsTransferRefrenceExistsAsync(string transactionReference)
        {
            return await _context.Transfers.AnyAsync(t => t.TransferReference == transactionReference);
        }

        public async Task AccountPaymentAsync(AccountPayment accountPayment)
        {
            await _context.AccountPayments.AddAsync(accountPayment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AccountPayment>> GetAccountPayments(string status)
        {
            return await _context.AccountPayments
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<AccountPayment> GetAccountPaymentById(int id)
        {
            return await _context.AccountPayments
                .OrderByDescending(t => t.Date)
                .FirstAsync(t => t.Id == id);
        }

        public async Task ConfirmAccountPayment(AccountPayment accountPayment)
        {
            _context.AccountPayments.Update(accountPayment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAccountPaymentRecordAsync(int id)
        {
            var user = await _context.AccountPayments.FindAsync(id);
            if (user != null)
            {
                _context.AccountPayments.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ThirdPartyPaymentAsync(ThirdPartyPayment thirdPartyPayment)
        {
            await _context.ThirdPartyPayments.AddAsync(thirdPartyPayment);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ThirdPartyPayment>> GetThirdPartyTransfers(string status)
        {
            return await _context.ThirdPartyPayments
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<ThirdPartyPayment> GetThirdPartyPaymentById(int id)
        {
            return await _context.ThirdPartyPayments
                .OrderByDescending(t => t.Date)
                .FirstAsync(t => t.Id == id);
        }

        public async Task ThirdPartyTransferCompleted(ThirdPartyPayment thirdPartyPayment)
        {
            _context.ThirdPartyPayments.Update(thirdPartyPayment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteThirdPartyTransferRecordAsync(int id)
        {
            var user = await _context.ThirdPartyPayments.FindAsync(id);
            if (user != null)
            {
                _context.ThirdPartyPayments.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task TopUpAccountAsync(AccountTopUp accountTopUp)
        {
            try
            {
                await _context.AccountTopUps.AddAsync(accountTopUp);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving top up: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task<List<AccountTopUp>> GetAccountTopUpsAsync(string status)
        {
            return await _context.AccountTopUps
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.TransactionDate)
                .Include(t => t.TransferDetails)
                .Include(t => t.TransferEvidence)
                .ToListAsync();
        }

        public async Task<AccountTopUp> GetUserAccountTopUpAsync(int id)
        {
            return await _context.AccountTopUps
                .OrderByDescending(t => t.TransactionDate)
                .Include(t => t.TransferDetails)
                .Include(e => e.TransferEvidence)
                .FirstAsync(t => t.Id == id);
        }
        
        public async Task ConfirmAccountTopUp(AccountTopUp accountTopUp)
        {
            _context.AccountTopUps.Update(accountTopUp);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAccountTopUpRecordAsync(int id)
        {
            var user = await _context.AccountTopUps.FindAsync(id);
            if (user != null)
            {
                _context.AccountTopUps.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Transfer>> GetTransferHistory(string email)
        {
            return await _context.Transfers
                .Where(t => t.Email == email)
                .OrderByDescending(t => t.TransactionDate)
                .Include(t => t.BankDetails)
                .ToListAsync();
        }

        public async Task<List<AccountPayment>> GetAccountPaymentHistory(string email)
        {
            return await _context.AccountPayments
                .Where(t => t.Email == email)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<List<ThirdPartyPayment>> GetThirdPartyHistory(string email)
        {
            return await _context.ThirdPartyPayments
                .Where(t => t.Email == email)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<List<AccountTopUp>> GetAccountTopUpHistory(string email)
        {
            return await _context.AccountTopUps
                .Where(t => t.Email == email)
                .OrderByDescending(t => t.TransactionDate)
                .Include(t => t.TransferDetails)
                .ToListAsync();
        }

        public async Task<Transfer> GetRemitStatus(string email)
        {
            return null;
        }
    }
}

