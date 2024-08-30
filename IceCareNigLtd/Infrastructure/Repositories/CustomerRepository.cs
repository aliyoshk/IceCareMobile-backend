using System;
using Microsoft.EntityFrameworkCore;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Infrastructure.Data;
using IceCareNigLtd.Infrastructure.Interfaces;

namespace IceCareNigLtd.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Customer> GetCustomerByEmailAsync(string phone)
        {
            return await _context.Customers.FirstOrDefaultAsync(e => e.PhoneNumber == phone);
        }

        public async Task<List<Customer>> GetCustomersAsync()
        {
            return await _context.Customers
                .Include(c => c.Banks)
                .ToListAsync();
        }

        public async Task<int> GetCustomersCountAsync()
        {
            return await _context.Customers.CountAsync();
        }

        public async Task<decimal> GetTotalTransferredAmountAsync()
        {
            return await _context.Customers.SumAsync(c => c.TotalNairaAmount);
        }
    }
}

