﻿// <auto-generated />
using System;
using IceCareNigLtd.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace IceCareNigLtd.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250101135402_fixes")]
    partial class fixes
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.Property<string>("BankName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("EntityName")
                        .IsRequired()
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

                    b.Property<int>("SupplierId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

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

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.CompanyPhones", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("SettingsId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SettingsId");

                    b.ToTable("CompanyPhones");
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

                    b.Property<decimal>("Deposit")
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

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("TotalNairaAmount")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.CustomerBankInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("AmountTransferred")
                        .HasColumnType("TEXT");

                    b.Property<string>("BankName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("CustomerId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("CustomerBankInfo");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.CustomerPaymentReceipt", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CustomerId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Reciept")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("CustomerPaymentReceipt");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.DollarAvailable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("DollarBalance")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("DollarAvailables");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Balance")
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Deposit")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("DollarAmount")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Settings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

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

                    b.Property<decimal>("Balance")
                        .HasColumnType("TEXT");

                    b.Property<int>("Channel")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Deposit")
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

                    b.Property<decimal>("Amount")
                        .HasColumnType("TEXT");

                    b.Property<int>("Category")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Channel")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Currency")
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

                    b.Property<string>("ReferenceNo")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AccountPayments");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Users.AccountTopUp", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AccountNo")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Approver")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Category")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Currency")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Reference")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AccountTopUps");
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

                    b.Property<string>("Approver")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("BankName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Category")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Channel")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CustomerAccount")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ReferenceNo")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("ThirdPartyPayments");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Users.TopUpEvidence", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AccountTopUpId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Receipts")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AccountTopUpId");

                    b.ToTable("TopUpEvidences");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Users.Transfer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Approver")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Category")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Channel")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Currency")
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

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("TransferReference")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Transfers");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Users.TransferBank", b =>
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

                    b.Property<int>("TransferId")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("TransferredAmount")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("TransferId");

                    b.ToTable("TransferBanks");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Users.TransferDetail", b =>
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

                    b.Property<int>("AccountTopUpId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("BankName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("TransferredAmount")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AccountTopUpId");

                    b.ToTable("TransferDetails");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Users.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AccountNumber")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<decimal>("BalanceDollar")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("BalanceNaira")
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

                    b.Property<string>("Reviewer")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.BankInfo", b =>
                {
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

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.CompanyPhones", b =>
                {
                    b.HasOne("IceCareNigLtd.Core.Entities.Settings", null)
                        .WithMany("CompanyPhoneNumbers")
                        .HasForeignKey("SettingsId");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.CustomerBankInfo", b =>
                {
                    b.HasOne("IceCareNigLtd.Core.Entities.Customer", "Customer")
                        .WithMany("Banks")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.CustomerPaymentReceipt", b =>
                {
                    b.HasOne("IceCareNigLtd.Core.Entities.Customer", "Customer")
                        .WithMany("PaymentEvidence")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");
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

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Users.TopUpEvidence", b =>
                {
                    b.HasOne("IceCareNigLtd.Core.Entities.Users.AccountTopUp", "AccountTopUp")
                        .WithMany("TransferEvidence")
                        .HasForeignKey("AccountTopUpId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccountTopUp");
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

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Users.TransferDetail", b =>
                {
                    b.HasOne("IceCareNigLtd.Core.Entities.Users.AccountTopUp", "AccountTopUp")
                        .WithMany("TransferDetails")
                        .HasForeignKey("AccountTopUpId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccountTopUp");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Customer", b =>
                {
                    b.Navigation("Banks");

                    b.Navigation("PaymentEvidence");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Settings", b =>
                {
                    b.Navigation("CompanyAccounts");

                    b.Navigation("CompanyPhoneNumbers");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Supplier", b =>
                {
                    b.Navigation("Banks");
                });

            modelBuilder.Entity("IceCareNigLtd.Core.Entities.Users.AccountTopUp", b =>
                {
                    b.Navigation("TransferDetails");

                    b.Navigation("TransferEvidence");
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
