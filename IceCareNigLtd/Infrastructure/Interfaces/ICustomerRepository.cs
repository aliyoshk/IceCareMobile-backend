using System;
using IceCareNigLtd.Core.Entities;

namespace IceCareNigLtd.Infrastructure.Interfaces
{
	public interface ICustomerRepository
	{
        Task AddCustomerAsync(Customer customer);
        Task<List<Customer>> GetCustomersAsync();
        Task<int> GetCustomersCountAsync();
        Task<decimal> GetTotalTransferredAmountAsync();
    }
}

