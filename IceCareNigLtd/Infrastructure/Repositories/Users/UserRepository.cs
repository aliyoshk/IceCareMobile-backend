﻿using System;
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

        public async Task<Registration> GetUserByIdAsync(int userId)
        {
            return await _context.Registrations.FindAsync(userId);
        }

        public async Task<Registration> GetUserByEmailAsync(string email)
        {
            return await _context.Registrations
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<List<Registration>> GetUsersByStatusAsync(string status)
        {
            return await _context.Registrations
                .Where(u => u.Status == status)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task AddUserAsync(Registration user)
        {
            await _context.Registrations.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(Registration user)
        {
            _context.Registrations.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task MoveToApprovedAsync(Registration user)
        {
            user.Status = "Approved";
            _context.Registrations.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task MoveToRejectedAsync(Registration user)
        {
            user.Status = "Rejected";
            _context.Registrations.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _context.Registrations.FindAsync(userId);
            if (user != null)
            {
                _context.Registrations.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsAccountNumberExistsAsync(string accountNumber)
        {
            return await _context.Registrations.AnyAsync(u => u.AccountNumber == accountNumber);
        }

        public async Task<bool> IsPhoneNumberExistsAsync(string phoneNumber)
        {
            return await _context.Registrations.AnyAsync(p => p.Phone == phoneNumber);
        }

        public async Task ResetPasswordAsync(Registration user)
        {
            var userDetail = await _context.Registrations.FindAsync(user.Id);
            if (userDetail != null)
            {
                userDetail.Password = user.Password;
                _context.Registrations.Update(userDetail);
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

            //return await _context.Transfers.FindAsync((id));
        }

        public async Task<List<Transfer>> GetTransferByStatusAsync(string status)
        {
            //return await _context.Transfers.Where(t => t.Status == status).ToListAsync();
            return await _context.Transfers
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.TransactionDate)
                .Include(t => t.BankDetails)
                .Include(t => t.TransferEvidence)
                .ToListAsync();
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

        public async Task ThirdPartyPaymentAsync(ThirdPartyPayment thirdPartyPayment)
        {
            await _context.ThirdPartyPayments.AddAsync(thirdPartyPayment);
            await _context.SaveChangesAsync();
        }

        public async Task SubtractNairaTransferAmountAsync(string email, decimal amount)
        {
            var user = await _context.Transfers.FindAsync(email);
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

                _context.Transfers.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SubtractDollarTransferAmountAsync(string email, decimal amount)
        {
            var user = await _context.Transfers.FindAsync(email);
            if (user != null)
            {
                if (amount <= 0)
                    return;

                if (user.DollarAmount >= amount)
                {
                    user.DollarAmount -= amount;
                }
                else
                    return;

                _context.Transfers.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddNairaTransferAmountAsync(string email, decimal amount)
        {
            var user = await _context.Transfers.FindAsync(email);
            if (user != null)
            {
                if (amount <= 0)
                    return;

                user.DollarAmount += amount;

                _context.Transfers.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddDollarTransferAmountAsync(string email, decimal amount)
        {
            var user = await _context.Transfers.FindAsync(email);
            if (user != null)
            {
                if (amount <= 0)
                    return;

                user.DollarAmount += amount;

                _context.Transfers.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteCustomerTransferRecordAsync(int userId)
        {
            var user = await _context.Transfers.FindAsync(userId);
            if (user != null)
            {
                _context.Transfers.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ThirdPartyPayment>> GetThirdPartyTransfers() => await _context.ThirdPartyPayments.ToListAsync();

        public async Task<ThirdPartyPayment> GetThirdPartyPaymentById(int id)
        {
            return await _context.ThirdPartyPayments.FindAsync(id);
        }

        public async Task ThirdPartyTransferCompleted(ThirdPartyPayment thirdPartyPayment)
        {
            _context.ThirdPartyPayments.Update(thirdPartyPayment);
            await _context.SaveChangesAsync();
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

        public async Task<List<AccountTopUp>> GetAccountTopUpsAsync()
        {
            //return await _context.AccountTopUps.ToListAsync();
            return await _context.AccountTopUps
                .OrderByDescending(t => t.TransactionDate)
                .Include(t => t.TransferDetails)
                .Include(t => t.TransferEvidence)
                .ToListAsync();
        }

        public async Task<AccountTopUp> GetUserAccountTopUpAsync(int id) => await _context.AccountTopUps.FindAsync(id);
        

        public async Task ConfirmAccountTopUp(AccountTopUp accountTopUp)
        {
            _context.AccountTopUps.Update(accountTopUp);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Transfer>> GetTransactionHistory(string email)
        {
            return await _context.Transfers
                .Where(t => t.Email == email)
                .OrderByDescending(t => t.TransactionDate)
                .Include(t => t.BankDetails)
                .ToListAsync();
        }

        public Task<Transfer> GetRemitStatus(string email)
        {
            return null;
        }
    }
}

