using System;
using IceCareNigLtd.Api.Models;
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

        public async Task<Response<CustomerDto>> AddCustomerAsync(CustomerDto customerDto)
        {
            // Calculate the total dollar amount available from all suppliers
            var totalSupplierDollarAmount = await _supplierRepository.GetTotalDollarAmountAsync();
            // Check if the total supplier dollar amount is sufficient for the customer's dollar amount
            if (totalSupplierDollarAmount < customerDto.DollarAmount)
            {
                return new Response<CustomerDto>
                {
                    Success = false,
                    Message = "Insufficient supplier dollar amount to fulfill the customer request",
                    Data = null
                };
            }
            await _supplierRepository.SubtractDollarAmountAsync(customerDto.DollarAmount);


            var totalDollarAmount = customerDto.DollarAmount;
            var totalNairaAmount = customerDto.TotalNairaAmount;

            var customer = new Customer
            {
                Name = customerDto.Name,
                PhoneNumber = customerDto.PhoneNumber,
                Date = customerDto.Date,
                ModeOfPayment = Enum.Parse<ModeOfPayment>(customerDto.ModeOfPayment.ToString()),
                DollarRate = customerDto.DollarRate,
                DollarAmount = customerDto.DollarAmount,
                TotalDollarAmount = totalDollarAmount,
                TotalNairaAmount = totalNairaAmount,
                Banks = customerDto.Banks.Select(b => new BankInfo
                {
                    BankName = b.BankName,
                    AmountTransferred = b.AmountTransferred,
                }).ToList()
            };

            await _customerRepository.AddCustomerAsync(customer);

            
            // Register the bank details with the bank module
            foreach (var bankInfo in customerDto.Banks)
            {
                var bank = new Bank
                {
                    BankName = Enum.Parse<BankName>(bankInfo.BankName.ToString()),
                    Date = DateTime.UtcNow,
                    PersonType = PersonType.Customer,
                    ExpenseType = ExpenseType.Credit,
                    Amount = bankInfo.AmountTransferred,
                };

                await _bankRepository.AddBankAsync(bank);
            }

            return new Response<CustomerDto>
            {
                Success = true,
                Message = "Customer added successfully",
                Data = customerDto
            };
        }

        public async Task<Response<List<CustomerDto>>> GetCustomersAsync()
        {
            var customers = await _customerRepository.GetCustomersAsync();
            var customerDtos = customers.Select(c => new CustomerDto
            {
                Name = c.Name,
                PhoneNumber = c.PhoneNumber,
                Date = c.Date,
                ModeOfPayment = c.ModeOfPayment.ToString(),
                DollarRate = c.DollarRate,
                DollarAmount = c.DollarAmount,
                TotalDollarAmount = c.TotalDollarAmount,
                TotalNairaAmount = c.TotalNairaAmount,
                Banks = c.Banks.Select(b => new BankInfoDto
                {
                    BankName = b.BankName.ToString(),
                    AmountTransferred = b.AmountTransferred,
                }).ToList()
            }).ToList();

            // Calculate the total number of customers and the total amount transferred
            var totalCustomers = customerDtos.Count;
            //var totalAmountTransferred = customerDtos.Sum(c => c.Banks.Sum(b => b.AmountTransferred));
            var responseDto = new CustomersResponseDto
            {
                Customers = customerDtos,
                TotalCustomers = totalCustomers,
                //TotalAmountTransferred = totalAmountTransferred
            };

            return new Response<List<CustomerDto>>
            {
                Success = true,
                Message = "Customers retrieved successfully",
                Data = customerDtos
            };
        }
    }
}

