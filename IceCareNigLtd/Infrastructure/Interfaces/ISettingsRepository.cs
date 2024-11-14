using System;
using IceCareNigLtd.Core.Entities;

namespace IceCareNigLtd.Infrastructure.Interfaces
{
	public interface ISettingsRepository
	{
        Task<decimal> GetDollarRateAsync();
        Task<bool> UpdateDollarRateAsync(decimal newDollarRate);
        Task<List<CompanyPhones>> GetCompanyPhoneNumbersAsync();
        Task AddCompanyPhoneNumbersAsync(CompanyPhones phoneNumber);
        Task<List<CompanyAccounts>> GetCompanyAccountsAsync();
        Task AddCompanyAccountAsync(CompanyAccounts accounts);
        Task<bool> DeleteAccountAsync(int bankId);
    }
}

