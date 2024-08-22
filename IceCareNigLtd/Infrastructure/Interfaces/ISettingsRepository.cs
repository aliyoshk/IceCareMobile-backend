using System;
namespace IceCareNigLtd.Infrastructure.Interfaces
{
	public interface ISettingsRepository
	{
        Task<decimal> GetDollarRateAsync();
        Task<bool> UpdateDollarRateAsync(decimal newDollarRate);
        Task<string> GetCompanyPhoneNumbersAsync();
        Task<bool> UpdateCompanyPhoneNumbersAsync(List<string> phoneNumbers);
    }
}

