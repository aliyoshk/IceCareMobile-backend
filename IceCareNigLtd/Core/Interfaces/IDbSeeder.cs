using System;
using IceCareNigLtd.Infrastructure.Data;

namespace IceCareNigLtd.Core.Interfaces
{
	public interface IDbSeeder
	{
        Task SeedAsync(ApplicationDbContext context);
    }
}

