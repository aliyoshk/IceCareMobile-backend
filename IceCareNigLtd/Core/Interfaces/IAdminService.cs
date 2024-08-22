using System;
using IceCareNigLtd.Api.Models;

namespace IceCareNigLtd.Core.Interfaces
{
	public interface IAdminService
	{
        Task<Response<AdminDto>> AddAdminAsync(AdminDto adminDto);
        Task<Response<List<AdminDto>>> GetAdminsAsync();
        Task<Response<object>> DeleteAdminAsync(int adminId);
        Task<Response<string>> LoginAsync(AdminLoginDto adminLoginDto);
    }
}

