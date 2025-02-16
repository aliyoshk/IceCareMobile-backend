using System;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Core.Entities.Users;
using IceCareNigLtd.Infrastructure.Data;
using IceCareNigLtd.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IceCareNigLtd.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAdminAsync(Admin admin)
        {
            try
            {
                await _context.Admins.AddAsync(admin);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding admin: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task<List<Admin>> GetAdminsAsync()
        {
            return await _context.Admins.OrderByDescending(c => c.Date).ToListAsync();
        }

        public async Task DeleteAdminAsync(int adminId)
        {
            var admin = await _context.Admins.FindAsync(adminId);
            if (admin != null)
            {
                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Admin> GetAdminByUsernameAsync(string username)
        {
            return await _context.Admins.FirstOrDefaultAsync(a => a.Email.ToLower() == username.ToLower());
        }
    }
}

