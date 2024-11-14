using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Core.Entities;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Infrastructure.Interfaces
{
	public interface IBankRepository
	{
        Task AddBankAsync(Bank bank);
        Task<List<Bank>> GetBanksAsync();
        Task<List<Bank>> GetBankRecordByNameAsync(string bankName);
        Task DeleteBankAsync(int bankId);
    }
}

