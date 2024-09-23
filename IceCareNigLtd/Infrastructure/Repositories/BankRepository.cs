using System;
using System.Net.NetworkInformation;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Core.Entities.Users;
using IceCareNigLtd.Infrastructure.Data;
using IceCareNigLtd.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Infrastructure.Repositories
{
    public class BankRepository : IBankRepository
    {
        private readonly ApplicationDbContext _context;

        public BankRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddBankAsync(Bank bank)
        {
            await _context.Banks.AddAsync(bank);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Bank>> GetBanksAsync()
        {
            return await _context.Banks.ToListAsync();
        }

        public async Task<List<Bank>> GetBankRecordByNameAsync(string bankName)
        {
            var parsedBank = bankName.ToString();
            return await _context.Banks.Where(b => b.BankName == parsedBank).ToListAsync() ?? null;
        }

        public async Task DeleteBankAsync(int bankId)
        {
            var bank = await _context.Banks.FindAsync(bankId);
            if (bank != null)
            {
                _context.Banks.Remove(bank);
                await _context.SaveChangesAsync();
            }
        }
    }
}

