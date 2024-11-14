using System;
using System.Threading.Tasks;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Core.Entities;

namespace IceCareNigLtd.Infrastructure.Interfaces
{
	public interface ICustomerRepository
	{
        Task AddCustomerAsync(Customer customer);
        Task<List<Customer>> GetCustomersAsync();
        Task<Customer> GetCustomerByIdAsync(int id);
        Task<Customer> GetCustomerByEmailAsync(string phone);
        Task<int> GetCustomersCountAsync();
        Task<decimal> GetTotalTransferredAmountAsync();
        Task DeleteCustomerAsync(int customerId);
    }
}

