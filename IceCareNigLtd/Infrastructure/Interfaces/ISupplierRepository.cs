using System;
using IceCareNigLtd.Core.Entities;

namespace IceCareNigLtd.Infrastructure.Interfaces
{
    public interface ISupplierRepository
    {
        Task AddSupplierAsync(Supplier supplier);
        Task<List<Supplier>> GetSuppliersAsync();
        Task<decimal> GetTotalDollarAmountAsync();
        Task SubtractDollarAmountAsync(decimal amount);
        Task<int> GetSuppliersCountAsync();
        Task<decimal> GetTotalTransferredAmountAsync();
    }
}

