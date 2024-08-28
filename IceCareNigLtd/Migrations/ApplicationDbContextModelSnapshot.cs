﻿// <auto-generated />
using System;
using IceCareNigLtd.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace IceCareNigLtd.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.0");

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Admin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Admins");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Bank", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<int>("BankName")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int>("ExpenseType")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PersonType")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Banks");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.BankInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("AmountTransferred")
                        .HasColumnType("TEXT");

                    b.Property<string>("BankName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("CustomerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SupplierId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("SupplierId");

                    b.ToTable("BankInfo");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.CompanyAccounts", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AccountName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("AccountNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("BankName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("SettingsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SettingsId");

                    b.ToTable("CompanyAccounts");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AccountNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Balance")
                        .HasColumnType("TEXT");

                    b.Property<int>("Channel")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("DollarAmount")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("DollarRate")
                        .HasColumnType("TEXT");

                    b.Property<int>("ModeOfPayment")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("PaymentCurrency")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PaymentEvidence")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("TotalDollarAmount")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("TotalNairaAmount")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("DollarAmount")
                        .HasColumnType("TEXT");

                    b.Property<string>("ModeOfPayment")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Settings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CompanyPhoneNumbers")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("DollarRate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Supplier", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("DollarAmount")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("DollarRate")
                        .HasColumnType("TEXT");

                    b.Property<int>("ModeOfPayment")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("TotalDollarAmount")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("TotalNairaAmount")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Suppliers");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Users.AccountPayment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Balance")
                        .HasColumnType("TEXT");

                    b.Property<int>("Channel")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CustomerAccount")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("DollarAmount")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("NairaAmount")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AccountPayments");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Users.EvidenceOfTransfer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Receipts")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("TransferId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TransferId");

                    b.ToTable("EvidenceOfTransfers");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Users.Registration", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AccountNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("TEXT");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("TEXT");

                    b.Property<string>("ReviewedBy")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Registrations");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Users.ThirdPartyPayment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AccountName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("AccountNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Balance")
                        .HasColumnType("TEXT");

                    b.Property<string>("BankName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Channel")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CustomerAccount")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ThirdPartyPayments");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Users.Transfer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Balance")
                        .HasColumnType("TEXT");

                    b.Property<int>("Channel")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CustomerAccount")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("DollarAmount")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("DollarRate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Transfers");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Users.TransferBank", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BankName")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TransferId")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("TransferredAmount")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("TransferId");

                    b.ToTable("TransferBanks");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.BankInfo", b =>
                {
                    b.HasOne("IceCareNigLtd.Core.Entities.Customer", null)
                        .WithMany("Banks")
                        .HasForeignKey("CustomerId");

                    b.HasOne("IceCareNigLtd.Core.Entities.Supplier", "Supplier")
                        .WithMany("Banks")
                        .HasForeignKey("SupplierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.CompanyAccounts", b =>
                {
                    b.HasOne("IceCareNigLtd.Core.Entities.Settings", null)
                        .WithMany("CompanyAccounts")
                        .HasForeignKey("SettingsId");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Users.EvidenceOfTransfer", b =>
                {
                    b.HasOne("IceCareNigLtd.Core.Entities.Users.Transfer", "Transfer")
                        .WithMany("TransferEvidence")
                        .HasForeignKey("TransferId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Transfer");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Users.TransferBank", b =>
                {
                    b.HasOne("IceCareNigLtd.Core.Entities.Users.Transfer", "Transfer")
                        .WithMany("BankDetails")
                        .HasForeignKey("TransferId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Transfer");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Customer", b =>
                {
                    b.Navigation("Banks");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Settings", b =>
                {
                    b.Navigation("CompanyAccounts");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Supplier", b =>
                {
                    b.Navigation("Banks");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Users.Transfer", b =>
                {
                    b.Navigation("BankDetails");

                    b.Navigation("TransferEvidence");
                });
#pragma warning restore 612, 618
        }
    }
}
