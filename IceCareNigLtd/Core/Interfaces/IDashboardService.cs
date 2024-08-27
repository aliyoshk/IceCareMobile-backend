using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Core.Entities;

namespace IceCareNigLtd.Core.Interfaces
{
	public interface IDashboardService
	{
        Task<Response<DashboardDto>> GetDashboardDataAsync(string adminUsername);
        Task<Response<bool>> UpdateDollarRateAsync(decimal newDollarRate);
        Task<Response<bool>> UpdateCompanyPhoneNumbersAsync(List<string> phoneNumbers);
        Task<Response<bool>> UpdateAccountsAsync(List<CompanyAccounts> accounts);
    }
}

