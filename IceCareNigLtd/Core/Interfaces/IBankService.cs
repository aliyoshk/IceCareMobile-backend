using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Core.Entities;

namespace IceCareNigLtd.Core.Interfaces
{
    public interface IBankService
    {
        Task<Response<BankDto>> AddBankAsync(BankDto bankDto);
        Task<Response<List<BankDto>>> GetBanksAsync();
    }
}

