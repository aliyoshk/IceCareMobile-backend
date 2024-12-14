using System;
using Microsoft.EntityFrameworkCore;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Infrastructure.Data;
using IceCareNigLtd.Infrastructure.Interfaces;
using IceCareNigLtd.Api.Models;

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
                .Include(t => t.Banks)
                .Include(e => e.PaymentEvidence)
                .OrderByDescending(c => c.Date)
                .ToListAsync();
        }

        public async Task<int> GetCustomersCountAsync()
        {
            return await _context.Customers.CountAsync();
        }

        public async Task<decimal> GetTotalTransferredAmountAsync()
        {
            var totalAmount = await _context.Customers.SumAsync(c => (double)c.TotalNairaAmount);
            return (decimal)totalAmount;
        }

        public async Task DeleteCustomerAsync(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }
    }
}

