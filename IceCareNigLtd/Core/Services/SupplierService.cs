using System;
using IceCareNigLtd.Api.Models;
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

        public async Task<Response<SupplierDto>> AddSupplierAsync(SupplierDto supplierDto)
        {
            if (supplierDto.ModeOfPayment == ModeOfPayment.Transfer.ToString() && (supplierDto.Banks == null || !supplierDto.Banks.Any()))
            {
                return new Response<SupplierDto>
                {
                    Success = false,
                    Message = "Banks information is required when Mode of Payment is Transfer.",
                    Data = null
                };
            }

            if (supplierDto.ModeOfPayment == ModeOfPayment.Cash.ToString() && (supplierDto.Banks != null && supplierDto.Banks.Any()))
            {
                return new Response<SupplierDto>
                {
                    Success = false,
                    Message = "Banks information must be empty when Mode of Payment is Cash.",
                    Data = null
                };
            }

            var totalDollarAmount = supplierDto.TotalDollarAmount;
            var totalNairaAmount = supplierDto.TotalNairaAmount;

            var supplier = new Supplier
            {
                Name = supplierDto.Name,
                PhoneNumber = supplierDto.PhoneNumber,
                Date = DateTime.UtcNow,
                ModeOfPayment = Enum.Parse<ModeOfPayment>(supplierDto.ModeOfPayment.ToString()),
                DollarRate = supplierDto.DollarRate,
                DollarAmount = supplierDto.DollarAmount,
                TotalDollarAmount = totalDollarAmount,
                TotalNairaAmount = totalNairaAmount,
                Banks = supplierDto.Banks?.Select(b => new BankInfo
                {
                    BankName = b.BankName,
                    AmountTransferred = b.AmountTransferred,
                }).ToList() ?? new List<BankInfo>()
            };

            await _supplierRepository.AddSupplierAsync(supplier);

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

            return new Response<SupplierDto>
            {
                Success = true,
                Message = "Supplier added successfully",
                Data = supplierDto
            };
        }

        public async Task<Response<List<SupplierDto>>> GetSuppliersAsync()
        {
            var suppliers = await _supplierRepository.GetSuppliersAsync();
            var supplierDtos = suppliers.Select(s => new SupplierDto
            {
                Name = s.Name,
                PhoneNumber = s.PhoneNumber,
                Date = s.Date,
                ModeOfPayment = s.ModeOfPayment.ToString(),
                DollarRate = s.DollarRate,
                DollarAmount = s.DollarAmount,
                TotalDollarAmount = s.TotalDollarAmount,
                TotalNairaAmount = s.TotalNairaAmount,
                Banks = s.Banks.Select(b => new BankInfoDto
                {
                    BankName = b.BankName.ToString(),
                    AmountTransferred = b.AmountTransferred,
                }).ToList()
            }).ToList();

            // Calculate the total number of suppliers and the total amount transferred
            var totalCustomers = supplierDtos.Count;
            //var totalAmountTransferred = supplierDtos.Sum(c => c.Banks.Sum(b => b.AmountTransferred));
            var responseDto = new SuppliersResponseDto
            {
                Suppliers = supplierDtos,
                TotalCustomers = totalCustomers,
                //TotalAmountTransferred = totalAmountTransferred
            };

            return new Response<List<SupplierDto>>
            {
                Success = true,
                Message = "Suppliers retrieved successfully",
                Data = supplierDtos
            };
        }
    }
}

