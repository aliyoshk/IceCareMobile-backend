using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Users;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Core.Interfaces;
using IceCareNigLtd.Infrastructure.Interfaces;

namespace IceCareNigLtd.Core.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBankRepository _bankRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly ISettingsRepository _settingsRepository;


        public DashboardService(ISupplierRepository supplierRepository, ICustomerRepository customerRepository, IBankRepository bankRepository, IAdminRepository adminRepository, ISettingsRepository settingsRepository)
        {
            _supplierRepository = supplierRepository;
            _customerRepository = customerRepository;
            _bankRepository = bankRepository;
            _adminRepository = adminRepository;
            _settingsRepository = settingsRepository;
        }



        public async Task<Response<DashboardDto>> GetDashboardDataAsync(string adminUsername)
        {
            //this define for hardcoded admin
            //var hardcodedEmail = "aliyoshk@gmail.com";

            //// Check if the adminUsername matches the hardcoded email
            //if (adminUsername == hardcodedEmail)
            //{
            //    // Create a hardcoded admin object
            //    var hardcodedAdmin = new Admin
            //    {
            //        Name = "Hardcoded Admin",
            //        Email = hardcodedEmail,
            //        Role = "Super",
            //        // No need to set Password or Date here
            //    };

            //    // Number of Suppliers
            //    var numberOfSupplier = await _supplierRepository.GetSuppliersCountAsync();

            //    // Number of Customers
            //    var numberOfCustomer = await _customerRepository.GetCustomersCountAsync();

            //    // Total Transferred Amount by summing supplier and customer transfer amounts
            //    var totalSupplierTransfer = await _supplierRepository.GetTotalTransferredAmountAsync();
            //    var totalCustomerTransfer = await _customerRepository.GetTotalTransferredAmountAsync();
            //    var totalTransferredAmounts = totalSupplierTransfer - totalCustomerTransfer;

            //    // Available Dollar Amount based on the saved total dollar from supplier
            //    var availableDollarAmounts = await _supplierRepository.GetTotalDollarAmountAsync();

            //    // Get current Dollar Rate
            //    var dollarRates = await _settingsRepository.GetDollarRateAsync();

            //    // Get Company Phone Numbers
            //    var phoneNumber = await _settingsRepository.GetCompanyPhoneNumbersAsync();

            //    // Populate the DTO
            //    var dashboardDtos = new DashboardDto
            //    {
            //        AdminName = hardcodedAdmin.Name,
            //        AdminRole = hardcodedAdmin.Role,
            //        NumberOfSuppliers = numberOfSupplier,
            //        NumberOfCustomers = numberOfCustomer,
            //        TotalTransferredAmount = totalTransferredAmounts,
            //        AvailableDollarAmount = availableDollarAmounts,
            //        DollarRate = dollarRates,
            //        CompanyPhoneNumbers = phoneNumber,
            //        ShowAdminPanel = hardcodedAdmin.Role != "Normal" ? true : false
            //    };

            //    return new Response<DashboardDto>
            //    {
            //        Success = true,
            //        Message = "Dashboard data retrieved successfully",
            //        Data = dashboardDtos
            //    };
            //}


            // Get Admin Details
            var admin = await _adminRepository.GetAdminByUsernameAsync(adminUsername);
            if (admin == null)
            {
                return new Response<DashboardDto>
                {
                    Success = false,
                    Message = "Admin not found"
                };
            }

            // Number of Suppliers
            var numberOfSuppliers = await _supplierRepository.GetSuppliersCountAsync();

            // Number of Customers
            var numberOfCustomers = await _customerRepository.GetCustomersCountAsync();

            // Total Transferred Amount by summing supplier and customer transfer amounts
            var totalSupplierTransfers = await _supplierRepository.GetTotalTransferredAmountAsync();
            var totalCustomerTransfers = await _customerRepository.GetTotalTransferredAmountAsync();
            var totalTransferredAmount = totalSupplierTransfers - totalCustomerTransfers;

            // Available Dollar Amount based on the saved total dollar from supplier
            var availableDollarAmount = await _supplierRepository.GetTotalDollarAmountAsync();

            // Get current Dollar Rate
            var dollarRate = await _settingsRepository.GetDollarRateAsync();

            // Get Company Phone Numbers
            var phoneNumbers = await _settingsRepository.GetCompanyPhoneNumbersAsync();

            //Get Company Accounts
            var accounts = await _settingsRepository.GetCompanyAccountsAsync();

            // Populate the DTO
            var dashboardDto = new DashboardDto
            {
                AdminName = admin.Name,
                AdminRole = admin.Role,
                NumberOfSuppliers = numberOfSuppliers,
                NumberOfCustomers = numberOfCustomers,
                TotalTransferredAmount = totalTransferredAmount,
                AvailableDollarAmount = availableDollarAmount,
                DollarRate = dollarRate,
                CompanyPhoneNumbers = phoneNumbers,
                ShowAdminPanel =  admin.Role.ToLower() != "normal"  ? true : false,
                CompanyAccounts = accounts
            };

            return new Response<DashboardDto>
            {
                Success = true,
                Message = "Dashboard data retrieved successfully",
                Data = dashboardDto
            };
        }

        public async Task<Response<bool>> UpdateDollarRateAsync(decimal newDollarRate)
        {
            var result = await _settingsRepository.UpdateDollarRateAsync(newDollarRate);
            if (!result)
            {
                return new Response<bool>
                {
                    Success = false,
                    Message = "Failed to update dollar rate"
                };
            }

            return new Response<bool>
            {
                Success = true,
                Message = "Dollar rate updated successfully",
                Data = true
            };
        }

        public async Task<Response<bool>> UpdateCompanyPhoneNumbersAsync(List<string> phoneNumbers)
        {
            var result = await _settingsRepository.UpdateCompanyPhoneNumbersAsync(phoneNumbers);
            if (!result)
            {
                return new Response<bool>
                {
                    Success = false,
                    Message = "Failed to update company phone numbers"
                };
            }

            return new Response<bool>
            {
                Success = true,
                Message = "Company phone numbers updated successfully",
                Data = true
            };
        }

        public async Task<Response<bool>> UpdateCompanyAccountsAsync(List<string> phoneNumbers)
        {
            var result = await _settingsRepository.UpdateCompanyPhoneNumbersAsync(phoneNumbers);
            if (!result)
            {
                return new Response<bool>
                {
                    Success = false,
                    Message = "Failed to update company phone numbers"
                };
            }

            return new Response<bool>
            {
                Success = true,
                Message = "Company phone numbers updated successfully",
                Data = true
            };
        }

        public async Task<Response<bool>> AddCompanyAccountAsyn(CompanyAccountsDto companyAccountsDto)
        {
           
            var data = new CompanyAccounts
            {
                AccountName = companyAccountsDto.AccountName,
                AccountNumber = companyAccountsDto.AccountNumber,
                BankName = companyAccountsDto.BankName
            };

            await _settingsRepository.AddCompanyAccountAsync(data);

            return new Response<bool> { Success = true, Message = "Account added successfully", Data = true };
        }

        public async Task<Response<bool>> DeleteAccountsAsync(int bankId)
        {
            var result = await _settingsRepository.DeleteAccountAsync(bankId);
            if (!result)
            {
                return new Response<bool>
                {
                    Success = false,
                    Message = "Account record doesn't exist"
                };
            }

            return new Response<bool>
            {
                Success = true,
                Message = "Account Deleted successfully",
                Data = true
            };
        }

        public async Task<Response<List<CompanyAccounts>>> GetCompanyAccountsAsync()
        {
            var accounts = await _settingsRepository.GetCompanyAccountsAsync();
            var data = accounts.Select(a => new CompanyAccounts
            {
                Id = a.Id,
                AccountName = a.AccountName,
                AccountNumber = a.AccountNumber,
                BankName = a.BankName
            }).ToList();

            return new Response<List<CompanyAccounts>> { Success = true, Message = "Accounts retrieved successfully", Data = data };
        }
    }
}

