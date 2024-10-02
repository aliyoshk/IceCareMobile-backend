﻿using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Users;
using IceCareNigLtd.Core.Entities;

namespace IceCareNigLtd.Core.Interfaces
{
	public interface IDashboardService
	{
        Task<Response<DashboardDto>> GetDashboardDataAsync(string adminUsername);
        Task<Response<string>> UpdateDollarRateAsync(UpdateDollarDto updateDollarDto);
        Task<Response<bool>> AddCompanyPhoneNumbersAsync(CompanyPhoneDto companyPhoneDto);
        Task<Response<List<CompanyAccounts>>> GetCompanyAccountsAsync();
        Task<Response<bool>> AddCompanyAccountAsyn(CompanyAccountsDto companyAccountsDto);
        Task<Response<bool>> DeleteAccountsAsync(int bankId);
    }
}

