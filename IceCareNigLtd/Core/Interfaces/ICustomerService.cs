using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Response;

namespace IceCareNigLtd.Core.Interfaces
{
    public interface ICustomerService
    {
        Task<Response<bool>> AddCustomerAsync(CustomerDto customerDto);
        Task<Response<CustomerResponse>> GetCustomersAsync();
    }
}

