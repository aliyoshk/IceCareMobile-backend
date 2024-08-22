using System;
using IceCareNigLtd.Core.Entities;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace IceCareNigLtd.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        // Define your DbSets (tables)
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Settings> Settings { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Supplier>()
                .HasMany(s => s.Banks)
                .WithOne(b => b.Supplier)
                .HasForeignKey(b => b.SupplierId);
        }
    }
}

