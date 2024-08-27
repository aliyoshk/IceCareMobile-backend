using System;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Infrastructure.Data;
using IceCareNigLtd.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IceCareNigLtd.Infrastructure.Repositories
{
    public class SettingsRepository : ISettingsRepository
    {
        private readonly ApplicationDbContext _context;

        public SettingsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetDollarRateAsync()
        {
            var settings = await _context.Settings.FirstOrDefaultAsync();
            return settings?.DollarRate ?? 0;
        }

        public async Task<bool> UpdateDollarRateAsync(decimal newDollarRate)
        {
            var settings = await _context.Settings.FirstOrDefaultAsync();
            if (settings == null)
            {
                return false;
            }

            settings.DollarRate = newDollarRate;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GetCompanyPhoneNumbersAsync()
        {
            var settings = await _context.Settings.FirstOrDefaultAsync();
            return settings?.CompanyPhoneNumbers ?? "";
        }

        public async Task<bool> UpdateCompanyPhoneNumbersAsync(List<string> phoneNumbers)
        {
            var settings = await _context.Settings.FirstOrDefaultAsync();
            if (settings == null)
            {
                return false;
            }

            settings.CompanyPhoneNumbers = string.Join(",", phoneNumbers);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CompanyAccounts>> GetCompanyAccountsAsync()
        {
            var accounts = await _context.CompanyAccounts.ToListAsync();
            if (accounts == null)
            {
                return new List<CompanyAccounts>(); ;
            }
            return accounts;
        }

        public async Task AddCompanyAccountAsync(CompanyAccounts accounts)
        {
            await _context.CompanyAccounts.AddAsync(accounts);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAccountAsync(int bankId)
        {
            var companyAccount = await _context.CompanyAccounts.FindAsync(bankId);
            if (companyAccount == null)
            {
                return false;
            }
            _context.CompanyAccounts.Remove(companyAccount);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

