using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Users;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Core.Interfaces
{
	public interface IAdminService
	{
        Task<Response<AdminDto>> AddAdminAsync(AdminDto adminDto);
        Task<Response<List<AdminDto>>> GetAdminsAsync();
        Task<Response<object>> DeleteAdminAsync(int adminId);
        Task<Response<string>> LoginAsync(AdminLoginDto adminLoginDto);


        // Add mobile onboarding 
        Task<Response<List<UserDto>>> GetUsersByStatusAsync(string status);
        Task<Response<string>> ChangeUserStatusAsync(ChangeUserStatusRequest request, string adminName = null);
        Task<Response<List<UserDto>>> GetApprovedUsersAsync();
        Task<Response<List<UserDto>>> GetRejectedUsersAsync();
    }
}

