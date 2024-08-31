using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Request;
using IceCareNigLtd.Api.Models.Response;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Core.Interfaces;
using IceCareNigLtd.Infrastructure.Interfaces;
using IceCareNigLtd.Infrastructure.Repositories;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Core.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IBankRepository _bankRepository;
        private readonly ISupplierRepository _supplierRepository;

        public CustomerService(ICustomerRepository customerRepository, IBankRepository bankRepository, ISupplierRepository supplierRepository)
        {
            _customerRepository = customerRepository;
            _bankRepository = bankRepository;
            _supplierRepository = supplierRepository;
        }

        public async Task<Response<bool>> AddCustomerAsync(CustomerDto customerDto)
        {
            var totalSupplierDollarAmount = await _supplierRepository.GetTotalDollarAmountAsync();

            if (totalSupplierDollarAmount < customerDto.DollarAmount)
            {
                return new Response<bool>
                {
                    Success = false,
                    Message = "Insufficient dollar to continue request",
                    Data = true
                };
            }
            
            decimal? total = customerDto.Banks.Sum(a => a.AmountTransferred);
            var totalNairaAmount = total ?? customerDto.Amount;

            var customer = new Customer
            {
                Name = customerDto.Name,
                PhoneNumber = customerDto.PhoneNumber,
                Date = DateTime.UtcNow,
                ModeOfPayment = Enum.Parse<ModeOfPayment>(customerDto.ModeOfPayment.ToString()),
                DollarRate = customerDto.DollarRate,
                DollarAmount = customerDto.DollarAmount,
                TotalNairaAmount = totalNairaAmount,
                Balance = customerDto.Balance,
                PaymentCurrency = Enum.Parse<PaymentCurrency>(customerDto.PaymentCurrency.ToString()),
                Channel = Channel.WalkIn,
                PaymentEvidence = customerDto.PaymentEvidence.Select(e => new CustomerPaymentReceipt
                {
                    Reciept = e.Receipt
                }).ToList(),
                AccountNumber = "N/A",
                Banks = customerDto.Banks.Select(b => new CustomerBankInfo
                {
                    BankName = b.BankName,
                    AmountTransferred = b.AmountTransferred,
                }).ToList()
            };

            await _supplierRepository.SubtractDollarAmountAsync(customerDto.DollarAmount);
            await _customerRepository.AddCustomerAsync(customer);

            
            foreach (var bankInfo in customerDto.Banks)
            {
                var bank = new Bank
                {
                    BankName = Enum.Parse<BankName>(bankInfo.BankName.ToString()),
                    Date = DateTime.UtcNow,
                    PersonType = PersonType.Customer,
                    ExpenseType = CreditType.Credit,
                    Amount = bankInfo.AmountTransferred,
                };

                await _bankRepository.AddBankAsync(bank);
            }

            return new Response<bool>
            {
                Success = true,
                Message = "Customer added successfully",
                Data = true
            };
        }

        public async Task<Response<CustomerResponse>> GetCustomersAsync()
        {
            var customers = await _customerRepository.GetCustomersAsync();
            var customerDtos = customers.Select(c => new CustomerResponseDto
            {
                Name = c.Name,
                PhoneNumber = c.PhoneNumber,
                Date = c.Date,
                ModeOfPayment = c.ModeOfPayment.ToString(),
                DollarRate = c.DollarRate,
                DollarAmount = c.DollarAmount,
                PaymentEvidence = c.PaymentEvidence.Select(e => new ReceiptDto
                {
                    Receipt = e.Reciept
                }).ToList(),
                Amount = c.TotalNairaAmount,
                Balance = c.Balance,
                PaymentCurrency = c.PaymentCurrency.ToString(),
                AccountNumber = c.AccountNumber,
                Banks = c.Banks.Select(b => new BankInfoDto
                {
                    BankName = b.BankName,
                    AmountTransferred = b.AmountTransferred,
                }).ToList(),
                
            }).ToList();

            var totalCustomers = customerDtos.Count();
            var totalDollarAmount = customerDtos.Sum(s => s.DollarAmount);
            var totalNairaAmount = customerDtos.Sum(s => s.Amount);

            var response = new CustomerResponse
            {
                TotalCustomers = totalCustomers,
                TotalDollarAmount = totalDollarAmount,
                TotalNairaAmount = totalNairaAmount,
                Customers = customerDtos
            };

            return new Response<CustomerResponse>
            {
                Success = true,
                Message = "Customers retrieved successfully",
                Data = response
            };
        }
    }
}

