using System;
using IceCareNigLtd.Core.Entities;

namespace IceCareNigLtd.Infrastructure.Interfaces
{
	public interface IAdminRepository
	{
        Task AddAdminAsync(Admin admin);
        Task<List<Admin>> GetAdminsAsync();
        Task DeleteAdminAsync(int adminId);
        Task<Admin> GetAdminByUsernameAsync(string username);
    }
}

