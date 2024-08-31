using System;
using Microsoft.EntityFrameworkCore;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Infrastructure.Data;
using IceCareNigLtd.Infrastructure.Interfaces;
using IceCareNigLtd.Core;
using IceCareNigLtd.Core.Entities.Users;

namespace IceCareNigLtd.Infrastructure.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly ApplicationDbContext _context;

        public SupplierRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddSupplierAsync(Supplier supplier)
        {
            await _context.Suppliers.AddAsync(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Supplier>> GetSuppliersAsync()
        {
            return await _context.Suppliers
                .Include(s => s.Banks)
                .ToListAsync();
        }

        public async Task<int> GetSuppliersCountAsync()
        {
            return await _context.Suppliers.CountAsync();
        }

        public async Task<decimal> GetTotalTransferredAmountAsync()
        {
            var totalAmount = await _context.Suppliers.SumAsync(s => (double)s.TotalNairaAmount);
            return (decimal)totalAmount;
        }

        public async Task<bool> SaveDollar(DollarAvailable dollarAvailable)
        {
            var dollars = await _context.DollarAvailables.FirstOrDefaultAsync();

            if (dollars != null)
            {
                dollars.DollarBalance += dollarAvailable.DollarBalance;
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                _context.DollarAvailables.Add(dollarAvailable);
                await _context.SaveChangesAsync();
                return true;
            }
        }
            

        public async Task<decimal> GetTotalDollarAmountAsync()
        {
            var totalDollarAmount = await _context.DollarAvailables.SumAsync(s => (double)s.DollarBalance);
            return (decimal)totalDollarAmount;
        }

        public async Task SubtractDollarAmountAsync(decimal amount)
        {
            var dollars = await _context.DollarAvailables.OrderBy(s => (double)s.DollarBalance).ToListAsync();

            foreach (var dollar in dollars)
            {
                if (amount <= 0)
                    break;

                var deduction = Math.Min(amount, dollar.DollarBalance);
                dollar.DollarBalance -= deduction;
                amount -= deduction;

                _context.DollarAvailables.Update(dollar);
            }

            await _context.SaveChangesAsync();
        }
    }
}

