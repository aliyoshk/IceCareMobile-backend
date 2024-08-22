using System;
using IceCareNigLtd.Api.Models;

namespace IceCareNigLtd.Core.Interfaces
{
    public interface ICustomerService
    {
        Task<Response<CustomerDto>> AddCustomerAsync(CustomerDto customerDto);
        Task<Response<List<CustomerDto>>> GetCustomersAsync();
    }
}

