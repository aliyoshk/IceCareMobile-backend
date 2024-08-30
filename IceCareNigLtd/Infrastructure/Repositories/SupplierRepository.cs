using System;
using Microsoft.EntityFrameworkCore;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Infrastructure.Data;
using IceCareNigLtd.Infrastructure.Interfaces;

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

        public async Task<decimal> GetTotalDollarAmountAsync()
        {
            var totalDollarAmount = await _context.Suppliers.SumAsync(s => (double)s.DollarAmount);
            return (decimal)totalDollarAmount;
        }

        public async Task SubtractDollarAmountAsync(decimal amount)
        {
            var suppliers = await _context.Suppliers.OrderBy(s => (double)s.DollarAmount).ToListAsync();

            foreach (var supplier in suppliers)
            {
                if (amount <= 0)
                    break;

                var deduction = Math.Min(amount, supplier.DollarAmount);
                supplier.DollarAmount -= deduction;
                amount -= deduction;

                _context.Suppliers.Update(supplier);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetSuppliersCountAsync()
        {
            return await _context.Suppliers.CountAsync();
        }

        public async Task<decimal> GetTotalTransferredAmountAsync()
        {
            return await _context.Suppliers.SumAsync(s => s.TotalNairaAmount);
        }
    }
}

