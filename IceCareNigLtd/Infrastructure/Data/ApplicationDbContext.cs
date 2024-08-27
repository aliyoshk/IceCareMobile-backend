using System;
using IceCareNigLtd.Core.Entities;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using IceCareNigLtd.Core.Entities.Users;

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
        public DbSet<CompanyAccounts> CompanyAccounts { get; set; }


        //Mobile app DBSets (table)
        public DbSet<Registration> Registrations { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Supplier>()
                .HasMany(s => s.Banks)
                .WithOne(b => b.Supplier)
                .HasForeignKey(b => b.SupplierId);


            modelBuilder.Entity<Registration>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.Phone).IsRequired().HasMaxLength(11);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Reason).HasMaxLength(500);
                entity.Property(e => e.AccountNumber).HasMaxLength(50);
            });
        }
    }
}

