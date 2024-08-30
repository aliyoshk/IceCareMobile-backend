using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Request;
using IceCareNigLtd.Api.Models.Response;

namespace IceCareNigLtd.Core.Interfaces
{
    public interface ISupplierService
    {
        Task<Response<bool>> AddSupplierAsync(SupplierRequest supplierDto);
        Task<Response<SupplierResponse>> GetSuppliersAsync();
    }
}

