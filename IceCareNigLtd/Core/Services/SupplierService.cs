using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Request;
using IceCareNigLtd.Api.Models.Response;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Core.Interfaces;
using IceCareNigLtd.Infrastructure.Interfaces;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Core.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IBankRepository _bankRepository;

        public SupplierService(ISupplierRepository supplierRepository, IBankRepository bankRepository)
        {
            _supplierRepository = supplierRepository;
            _bankRepository = bankRepository;
        }

        public async Task<Response<bool>> AddSupplierAsync(SupplierRequest supplierDto)
        {
            decimal? total = supplierDto.Banks.Sum(a => a.AmountTransferred);
            var totalNairaAmount = total ?? supplierDto.Amount;

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
                Banks = supplierDto.Banks?.Select(b => new BankInfo
                {
                    BankName = b.BankName,
                    AmountTransferred = b.AmountTransferred,
                }).ToList() ?? new List<BankInfo>() {  }
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
                    BankName = Enum.Parse<BankName>(bankInfo.BankName.ToString()),
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
                Name = s.Name,
                PhoneNumber = s.PhoneNumber,
                Date = s.Date,
                ModeOfPayment = s.ModeOfPayment.ToString(),
                DollarRate = s.DollarRate,
                DollarAmount = s.DollarAmount,
                Amount = s.TotalNairaAmount,
                Channel = s.Channel.ToString(),
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
    }
}

