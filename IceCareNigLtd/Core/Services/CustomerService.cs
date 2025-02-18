﻿using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Request;
using IceCareNigLtd.Api.Models.Response;
using IceCareNigLtd.Api.Models.Users;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Core.Entities.Users;
using IceCareNigLtd.Core.Interfaces;
using IceCareNigLtd.Infrastructure.Interfaces;
using IceCareNigLtd.Infrastructure.Interfaces.Users;
using IceCareNigLtd.Infrastructure.Repositories;
using IceCareNigLtd.Infrastructure.Repositories.Users;
using Microsoft.EntityFrameworkCore;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Core.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IBankRepository _bankRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IPaymentRepository _paymentsRepository;
        private readonly IUserRepository _usersRepository;

        public CustomerService(ICustomerRepository customerRepository, IBankRepository bankRepository, ISupplierRepository supplierRepository,
            ISettingsRepository settingsRepository, IPaymentRepository paymentsRepository, IUserRepository usersRepository)
        {
            _customerRepository = customerRepository;
            _bankRepository = bankRepository;
            _supplierRepository = supplierRepository;
            _settingsRepository = settingsRepository;
            _paymentsRepository = paymentsRepository;
            _usersRepository = usersRepository;
        }

        public async Task<Response<bool>> AddCustomerAsync(CustomerDto customerDto)
        {
            var totalSupplierDollarAmount = await _supplierRepository.GetTotalDollarAmountAsync();

            //if (totalSupplierDollarAmount < customerDto.DollarAmount)
            //{
            //    return new Response<bool>
            //    {
            //        Success = false,
            //        Message = "Insufficient dollar to continue request",
            //        Data = false
            //    };
            //}
            if (customerDto.Amount <= 0 && customerDto.ModeOfPayment == ModeOfPayment.Cash.ToString())
            {
                return new Response<bool>
                {
                    Success = false,
                    Message = "Amount should be greather than 0",
                    Data = false
                };
            }
            if (customerDto.ModeOfPayment == ModeOfPayment.Cash.ToString())
            {
                customerDto.Banks = new List<BankInfoDto>();
                customerDto.PaymentEvidence = new List<ReceiptDto>();
            }

            var accounts = await _settingsRepository.GetCompanyAccountsAsync();
            if (!accounts.Any())
                return new Response<bool> { Success = false, Message = "No bank record found" };

            var errorMessages = new List<string>();

            if (customerDto.ModeOfPayment == ModeOfPayment.Transfer.ToString())
            {
                customerDto.Amount = 0;
                foreach (var bank in customerDto.Banks)
                {
                    if (bank.BankName == "")
                        errorMessages.Add($"Select Bank, field cannot be empty");

                    if (bank.BankName == null)
                        errorMessages.Add($"{bank.BankName} doesn't exist in the system");

                    if (bank.AmountTransferred <= 0)
                        errorMessages.Add($"The amount transferred for {bank.BankName} should be greather than 0");
                }
            }

            if (errorMessages.Any())
                return new Response<bool> { Success = false, Message = string.Join("; ", errorMessages) };

            decimal total = customerDto.Banks.Sum(a => a.AmountTransferred);
            var totalNairaAmount = total > 0 ? total : customerDto.Amount;

            var customer = new Customer
            {
                Name = customerDto.Name,
                PhoneNumber = customerDto.PhoneNumber,
                Date = DateTime.UtcNow,
                ModeOfPayment = Enum.Parse<ModeOfPayment>(customerDto.ModeOfPayment.ToString()),
                DollarRate = customerDto.DollarRate,
                DollarAmount = customerDto.DollarAmount,
                TotalNairaAmount = totalNairaAmount,
                Balance = customerDto.Balance < 0 ? customerDto.Balance : 0,
                PaymentCurrency = Enum.Parse<PaymentCurrency>(customerDto.PaymentCurrency.ToString()),
                Channel = Channel.WalkIn,
                Deposit = customerDto.Balance > 0 ? customerDto.Balance : 0,
                PaymentEvidence = customerDto?.PaymentEvidence?.Select(e => new CustomerPaymentReceipt
                {
                    Reciept = e.Receipt
                }).ToList() ?? new List<CustomerPaymentReceipt>(),
                AccountNumber = "N/A",
                Banks = customerDto?.Banks?.Select(b => new CustomerBankInfo
                {
                    BankName = b.BankName,
                    AmountTransferred = b.AmountTransferred,
                }).ToList() ?? new List<CustomerBankInfo>()
            };

            //await _supplierRepository.SubtractDollarAmountAsync(customerDto.DollarAmount);
            await _customerRepository.AddCustomerAsync(customer);
            
            foreach (var bankInfo in customerDto.Banks)
            {
                var bank = new Bank
                {
                    EntityName = customerDto.Name,
                    BankName = bankInfo.BankName,
                    Date = DateTime.UtcNow,
                    PersonType = PersonType.Customer,
                    ExpenseType = CreditType.Credit,
                    Amount = bankInfo.AmountTransferred,
                };

                await _bankRepository.AddBankAsync(bank);
            }


            var isUserRegistered = await _usersRepository.IsPhoneNumberExistsAsync(customerDto.PhoneNumber);
            var userDetails = await _usersRepository.GetRegisteredUserByPhone(customerDto.PhoneNumber);
            //if (userDetails.FullName.ToLower().Split() == customerDto.Name.ToLower().Split())
            if (isUserRegistered && customerDto.PaymentCurrency == PaymentCurrency.Naira.ToString())
            {
                Category transactionCategory = Category.SingleBankPayment;
                if (customerDto.Banks.Count > 1)
                    transactionCategory = Category.MultipleBanksPayment;
                else if (customer.DollarAmount == 0)
                    transactionCategory = Category.AccountTopUp;
                var data = new Transfer
                {
                    TransactionDate = DateTime.UtcNow,
                    DollarAmount = customerDto.DollarAmount,
                    Description = "payment made via admin counter",
                    Channel = Channel.Web,
                    CustomerAccount = userDetails.AccountNumber,
                    Currency = PaymentCurrency.Naira,
                    CustomerName = userDetails.FullName,
                    Email = userDetails.Email,
                    DollarRate = customerDto.DollarRate,
                    TransferReference = "",
                    Status = "Confirmed",
                    Approver = "",
                    Category = transactionCategory,
                    PhoneNumber = customerDto.PhoneNumber,
                    BankDetails = null,
                    TransferEvidence = null
                };
                await _usersRepository.FundTransferAsync(data);

                var paidDollarQuantity = totalNairaAmount / customerDto.DollarRate;
                var remainingAmount = paidDollarQuantity - customerDto.DollarAmount;

                if (paidDollarQuantity > customerDto.DollarAmount)
                    await _usersRepository.AddUserNairaBalance(userDetails.Email, remainingAmount * customerDto.DollarRate);
                else if (customerDto.DollarAmount > paidDollarQuantity)
                    await _usersRepository.SubtractUserNairaBalance(userDetails.Email, (customerDto.DollarAmount - paidDollarQuantity) * customerDto.DollarRate);
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
                Id = c.Id,
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
                Deposit = c.Deposit,
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


        public async Task<Response<object>> DeleteCustomerAsync(int customerId)
        {
            await _customerRepository.DeleteCustomerAsync(customerId);
            return new Response<object> { Success = true, Message = "Customer deleted successfully" };
        }

        public async Task<Response<bool>> CompleteCustomerPayment(CompletePaymentRequest completePaymentRequest)
        {
            var customer =  await _customerRepository.GetCustomerByIdAsync(completePaymentRequest.CustomerId);
            var availableDollarAmount = await _supplierRepository.GetTotalDollarAmountAsync();

            if (completePaymentRequest.DollarAmount > availableDollarAmount)
                return new Response<bool> { Success = false, Message = "Insufficient dollar to complete request", Data = false };

            if (customer.Id != completePaymentRequest.CustomerId)
                return new Response<bool> { Success = false, Message = "Customer ID does not correspond with records.", Data = false };
            
            else if (customer.DollarAmount != completePaymentRequest.DollarAmount)
                return new Response<bool> { Success = false, Message = "Dollar amount does not match with saved record.", Data = false };

            else if (completePaymentRequest.DollarAmount == 0)
                return new Response<bool> { Success = false, Message = "Dollar amount should be greather than 0", Data = false };

            var payment = new Payment
            {
                CustomerName = customer.Name,
                Date = DateTime.UtcNow,
                DollarAmount = customer.DollarAmount + completePaymentRequest.Charges,
            };

            await _paymentsRepository.AddPaymentAsync(payment);
            await _supplierRepository.SubtractDollarAmountAsync(customer.DollarAmount);
            await _customerRepository.DeleteCustomerAsync(customer.Id);

            return new Response<bool>
            {
                Success = true,
                Message = "Customer payment is successful",
                Data = true
            };
        }
    }
}

