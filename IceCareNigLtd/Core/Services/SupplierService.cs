using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Network;
using IceCareNigLtd.Api.Models.Request;
using IceCareNigLtd.Api.Models.Response;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Core.Interfaces;
using IceCareNigLtd.Infrastructure.Interfaces;
using IceCareNigLtd.Infrastructure.Repositories;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Core.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IBankRepository _bankRepository;
        private readonly ISettingsRepository _settingsRepository;

        public SupplierService(ISupplierRepository supplierRepository, IBankRepository bankRepository, ISettingsRepository settingsRepository)
        {
            _supplierRepository = supplierRepository;
            _bankRepository = bankRepository;
            _settingsRepository = settingsRepository;
        }

        public async Task<Response<bool>> AddSupplierAsync(SupplierRequest supplierDto)
        {
            if (supplierDto.Amount <= 0 && supplierDto.ModeOfPayment == ModeOfPayment.Cash.ToString())
            {
                return new Response<bool>
                {
                    Success = false,
                    Message = "Amount should be greather than 0",
                    Data = false
                };
            }

            if (supplierDto.ModeOfPayment == ModeOfPayment.Cash.ToString())
                supplierDto.Banks = new List<BankInfoDto>();

            var accounts = await _settingsRepository.GetCompanyAccountsAsync();
            if (!accounts.Any())
                return new Response<bool> { Success = false, Message = "No bank record found" };

            var errorMessages = new List<string>();

            if (supplierDto.ModeOfPayment == ModeOfPayment.Transfer.ToString())
            {
                supplierDto.Amount = 0;
                foreach (var bank in supplierDto.Banks)
                {
                    if (bank.BankName == "")
                        errorMessages.Add($"Select bank, field cannot be empty");

                    if (bank.BankName == null)
                        errorMessages.Add($"{bank.BankName} doesn't exist in the system");

                    if (bank.AmountTransferred <= 0)
                        errorMessages.Add($"The amount transferred for {bank.BankName} should be greather than 0");
                }
            }

            if (errorMessages.Any())
                return new Response<bool> { Success = false, Message = string.Join("; ", errorMessages) };


            decimal total = supplierDto.Banks.Sum(a => a.AmountTransferred);
            var totalNairaAmount = total > 0 ? total : supplierDto.Amount;

            var supplier = new Supplier
            {
                Name = supplierDto.Name,
                PhoneNumber = supplierDto.PhoneNumber,
                Date = DateTime.UtcNow,
                ModeOfPayment = Enum.Parse<ModeOfPayment>(supplierDto.ModeOfPayment.ToString()),
                DollarRate = supplierDto.DollarRate,
                DollarAmount = supplierDto.DollarAmount,
                TotalNairaAmount = totalNairaAmount,
                Channel = Channel.WalkIn,
                Balance = supplierDto.Balance < 0 ? supplierDto.Balance : 0,
                Deposit = supplierDto.Balance > 0 ? supplierDto.Balance : 0,
                Banks = supplierDto.Banks?.Select(b => new BankInfo
                {
                    BankName = b.BankName ?? "",
                    AmountTransferred = b.AmountTransferred,
                }).ToList() ?? null
            };

            await _supplierRepository.AddSupplierAsync(supplier);


            var dollar = new DollarAvailable
            {
                DollarBalance = supplier.DollarAmount
            };
            await _supplierRepository.SaveDollar(dollar);

            // Register the bank details with the bank module
            foreach (var bankInfo in supplierDto.Banks)
            {
                var bank = new Bank
                {
                    EntityName = supplierDto.Name,
                    BankName = bankInfo.BankName.ToString(),
                    Date = DateTime.UtcNow,
                    PersonType = PersonType.Supplier,
                    ExpenseType = CreditType.Debit,
                    Amount = bankInfo.AmountTransferred,
                };

                await _bankRepository.AddBankAsync(bank);
            }

            return new Response<bool>
            {
                Success = true,
                Message = "Supplier added successfully",
                Data = true
            };
        }

        public async Task<Response<SupplierResponse>> GetSuppliersAsync()
        {
            var suppliers = await _supplierRepository.GetSuppliersAsync();

            // Map suppliers to SupplierRequest objects
            var supplierDtos = suppliers.Select(s => new SupplierRequest
            {
                Id = s.Id,
                Name = s.Name,
                PhoneNumber = s.PhoneNumber,
                Date = s.Date,
                ModeOfPayment = s.ModeOfPayment.ToString(),
                DollarRate = s.DollarRate,
                DollarAmount = s.DollarAmount,
                Amount = s.TotalNairaAmount,
                Channel = s.Channel.ToString(),
                Balance = s.Balance,
                Deposit = s.Deposit,
                Banks = s.Banks.Select(b => new BankInfoDto
                {
                    BankName = b.BankName.ToString(),
                    AmountTransferred = b.AmountTransferred,
                }).ToList()
            }).ToList();

            // Calculate aggregate totals
            var totalSuppliers = supplierDtos.Count();
            var totalDollarAmount = supplierDtos.Sum(s => s.DollarAmount);
            var totalNairaAmount = supplierDtos.Sum(s => s.Amount);

            // Create the final response DTO that includes the list and totals
            var responseDto = new SupplierResponse
            {
                Suppliers = supplierDtos,
                TotalSuppliers = totalSuppliers,
                TotalDollarAmount = totalDollarAmount,
                TotalNairaAmount = totalNairaAmount
            };

            return new Response<SupplierResponse>
            {
                Success = true,
                Message = "Suppliers retrieved successfully",
                Data = responseDto
            };
        }

        public async Task<Response<object>> DeleteSupplierAsync(int supplierId)
        {
            await _supplierRepository.DeleteSupplierAsync(supplierId);
            return new Response<object> { Success = true, Message = "Supplier deleted successfully" };
        }
    }
}

