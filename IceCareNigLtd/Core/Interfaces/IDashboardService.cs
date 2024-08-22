using System;
using IceCareNigLtd.Api.Models;

namespace IceCareNigLtd.Core.Interfaces
{
	public interface IDashboardService
	{
        Task<Response<DashboardDto>> GetDashboardDataAsync(string adminUsername);
        Task<Response<bool>> UpdateDollarRateAsync(decimal newDollarRate);
        Task<Response<bool>> UpdateCompanyPhoneNumbersAsync(List<string> phoneNumbers);
    }
}

