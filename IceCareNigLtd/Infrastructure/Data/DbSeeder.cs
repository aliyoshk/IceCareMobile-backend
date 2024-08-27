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
            if (!await context.Settings.AnyAsync())
            {
                context.Settings.Add(new Settings
                {
                    Id = 1,
                    DollarRate = 1.0m,
                    CompanyPhoneNumbers = "",
                    CompanyAccounts = new List<CompanyAccounts>
                    {
                        new CompanyAccounts
                        {
                            Id = 1,
                            AccountName = "Ice Care Nig Ltd",
                            AccountNumber = "0123456789",
                            BankName = "Wema Bank"
                        }
                    }
                });
                await context.SaveChangesAsync();
            }
        }
    }
}

