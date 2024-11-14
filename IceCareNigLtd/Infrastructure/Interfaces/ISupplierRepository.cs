using System;
using IceCareNigLtd.Core;
using IceCareNigLtd.Core.Entities;

namespace IceCareNigLtd.Infrastructure.Interfaces
{
    public interface ISupplierRepository
    {
        Task AddSupplierAsync(Supplier supplier);
        Task<List<Supplier>> GetSuppliersAsync();
        Task<int> GetSuppliersCountAsync();
        Task<decimal> GetTotalTransferredAmountAsync();
        Task<bool> SaveDollar(DollarAvailable dollarAvailable);
        Task<decimal> GetTotalDollarAmountAsync();
        Task SubtractDollarAmountAsync(decimal amount);
        Task DeleteSupplierAsync(int supplierId);
    }
}

