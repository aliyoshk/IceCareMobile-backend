using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Response;
using IceCareNigLtd.Core.Entities;

namespace IceCareNigLtd.Core.Interfaces
{
    public interface IBankService
    {
        Task<Response<BankDto>> AddBankAsync(BankDto bankDto);
        Task<Response<List<BankDto>>> GetBanksAsync();
        Task<Response<object>> DeleteBankAsync(int bankId);
        Task<Response<List<BankDto>>> GetBankRecordByNameAsync(string bankName);
    }
}

