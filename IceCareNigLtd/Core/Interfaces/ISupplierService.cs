using System;
using IceCareNigLtd.Api.Models;

namespace IceCareNigLtd.Core.Interfaces
{
    public interface ISupplierService
    {
        Task<Response<SupplierDto>> AddSupplierAsync(SupplierDto supplierDto);
        Task<Response<List<SupplierDto>>> GetSuppliersAsync();
    }
}

