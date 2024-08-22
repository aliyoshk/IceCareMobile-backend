using System;
using IceCareNigLtd.Core.Entities;

namespace IceCareNigLtd.Infrastructure.Interfaces
{
	public interface IBankRepository
	{
        Task AddBankAsync(Bank bank);
        Task<List<Bank>> GetBanksAsync();
    }
}

