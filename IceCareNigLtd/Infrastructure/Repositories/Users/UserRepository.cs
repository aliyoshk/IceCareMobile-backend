﻿using System;
using IceCareNigLtd.Core.Entities.Users;
using IceCareNigLtd.Infrastructure.Data;
using IceCareNigLtd.Infrastructure.Interfaces.Users;
using Microsoft.EntityFrameworkCore;

namespace IceCareNigLtd.Infrastructure.Repositories.Users
{
	public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Registration> GetUserByIdAsync(int userId)
        {
            return await _context.Registrations.FindAsync(userId);
        }

        public async Task<Registration> GetUserByEmailAsync(string email)
        {
            return await _context.Registrations
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        //public async Task<Registration> GetUserByPhoneAsync(string phone)
        //{
        //    return await _context.Registrations.AnyAsync(p => p.Phone == phone);
        //    return await _context.Registrations.AnyAsync(u => u.Phone == phone);
        //}

        public async Task<List<Registration>> GetUsersByStatusAsync(string status)
        {
            return await _context.Registrations
                .Where(u => u.Status == status)
                .ToListAsync();
        }

        public async Task AddUserAsync(Registration user)
        {
            await _context.Registrations.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(Registration user)
        {
            _context.Registrations.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task MoveToApprovedAsync(Registration user)
        {
            user.Status = "Approved";
            _context.Registrations.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task MoveToRejectedAsync(Registration user)
        {
            user.Status = "Rejected";
            _context.Registrations.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _context.Registrations.FindAsync(userId);
            if (user != null)
            {
                _context.Registrations.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsAccountNumberExistsAsync(string accountNumber)
        {
            return await _context.Registrations.AnyAsync(u => u.AccountNumber == accountNumber);
        }

        public async Task<bool> IsPhoneNumberExistsAsync(string phoneNumber)
        {
            return await _context.Registrations.AnyAsync(p => p.Phone == phoneNumber);
        }
    }
}

