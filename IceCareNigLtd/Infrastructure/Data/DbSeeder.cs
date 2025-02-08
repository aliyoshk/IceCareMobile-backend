using System;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IceCareNigLtd.Infrastructure.Data
{
    public class DbSeeder : IDbSeeder
    {
        public async Task SeedAsync(ApplicationDbContext context)
        {
            try
            {
                await context.Database.EnsureCreatedAsync();
                if (!await context.Settings.AnyAsync())
                {
                    context.Settings.Add(new Settings
                    {
                        Id = 1,
                        DollarRate = 1.0m
                    });
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database seeding failed: {ex.Message}");
                throw;
            }
        }
    }
}

