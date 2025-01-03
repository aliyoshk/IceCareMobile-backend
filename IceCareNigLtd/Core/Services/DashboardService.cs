using System;
using System.Linq;
using System.Net.NetworkInformation;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Users;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Core.Interfaces;
using IceCareNigLtd.Infrastructure.Interfaces;
using IceCareNigLtd.Infrastructure.Interfaces.Users;

namespace IceCareNigLtd.Core.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBankRepository _bankRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly ISettingsRepository _settingsRepository;
        private readonly IUserRepository _userRepository;


        public DashboardService(ISupplierRepository supplierRepository, ICustomerRepository customerRepository,
            IBankRepository bankRepository, IAdminRepository adminRepository, ISettingsRepository settingsRepository,
            IUserRepository userRepository)
        {
            _supplierRepository = supplierRepository;
            _customerRepository = customerRepository;
            _bankRepository = bankRepository;
            _adminRepository = adminRepository;
            _settingsRepository = settingsRepository;
            _userRepository = userRepository;
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
            var totalTransferredAmount =  totalCustomerTransfers - totalSupplierTransfers;

            // Available Dollar Amount based on the saved total dollar from supplier
            var availableDollarAmount = await _supplierRepository.GetTotalDollarAmountAsync();

            // Get current Dollar Rate
            var dollarRate = await _settingsRepository.GetDollarRateAsync();

            // Get Company Phone Numbers
            var phoneNumbers = await _settingsRepository.GetCompanyPhoneNumbersAsync();

            //Get Company Accounts
            var accounts = await _settingsRepository.GetCompanyAccountsAsync();

            var pendingTransfers = await _userRepository.GetTransferByStatusAsync("Pending");
            var pendingUsers = await _userRepository.GetRegisteredUserByStatus("Pending");

            var customers = await _customerRepository.GetCustomersAsync();
            var monthlyTransfers = customers
                .GroupBy(c => new { c.Date.Year, c.Date.Month })
                .Select(c => new MonthlyTransferDto
                {
                    Year = c.Key.Year,
                    Month = c.Key.Month,
                    TotalAmount = c.Sum(c => c.TotalNairaAmount)
                })
                .OrderBy(result => result.Year)
                .ThenBy(result => result.Month)
                .ToList();

            // Get the current day, month and year
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            var currentDay = DateTime.Today.Day;

            // Filter customers by the current month and year
            var filteredCustomersForCurrentMonth = customers
                .Where(c => c.Date.Month == currentMonth && c.Date.Year == currentYear)
                .ToList();

            // Filter customers by the current day, month, and year (for the current day)
            var filteredCustomersForCurrentDay = customers
                .Where(c => c.Date.Day == currentDay && c.Date.Month == currentMonth && c.Date.Year == currentYear)
                .ToList();

            // Calculate the total Naira and Dollar amounts for the current month
            var totalNairaAmountForCurrentMonth = filteredCustomersForCurrentMonth.Sum(c => c.TotalNairaAmount);
            var totalDollarAmountForCurrentMonth = filteredCustomersForCurrentMonth.Sum(c => c.DollarAmount);


            // Calculate the total Naira and Dollar amounts for the current day
            var totalNairaAmountForCurrentDay = filteredCustomersForCurrentDay.Sum(c => c.TotalNairaAmount);
            var totalDollarAmountForCurrentDay = filteredCustomersForCurrentDay.Sum(c => c.DollarAmount);

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
                TotalMonthlyNairaTransfer = totalNairaAmountForCurrentMonth,
                TotalMonthlyDollarSpent = totalDollarAmountForCurrentMonth,
                TotalDailyNairaTransfer = totalNairaAmountForCurrentDay,
                CompanyPhoneNumbers = phoneNumbers,
                ShowAdminPanel =  admin.Role.ToLower() != "normal"  ? true : false,
                CompanyAccounts = accounts,
                PendingTransfer = pendingTransfers.Take(4).Select(p => new PendingTransfer
                {
                    Name = p.CustomerName,
                    Date = p.TransactionDate,
                    amount = p.BankDetails.Sum(a => a.TransferredAmount)
                }).ToList(),
                PendingRegistration = pendingUsers.Take(4).Select(u => new PendingRegistration
                {
                    Name = u.FullName,
                    Date = u.Date
                }).ToList(),
                MonthlyTransfers = monthlyTransfers
            };

            return new Response<DashboardDto>
            {
                Success = true,
                Message = "Dashboard data retrieved successfully",
                Data = dashboardDto
            };
        }

        public async Task<Response<string>> UpdateDollarRateAsync(UpdateDollarDto updateDollarDto)
        {
            var result = await _settingsRepository.UpdateDollarRateAsync(updateDollarDto.NewDollarRate);
            if (!result)
            {
                return new Response<string>
                {
                    Success = false,
                    Message = "Failed to update dollar rate"
                };
            }

            return new Response<string>
            {
                Success = true,
                Message = "Dollar rate updated successfully",
            };
        }

        public async Task<Response<bool>> AddCompanyPhoneNumbersAsync(CompanyPhoneDto companyPhoneDto)
        {
            var result = await _settingsRepository.GetCompanyPhoneNumbersAsync();
            foreach (var item in result)
            {
                if (item.PhoneNumber == companyPhoneDto.phoneNumber)
                    return new Response<bool> { Success = false, Message = "Phone already exist in the system" };
            }

            var phoneNumber = new CompanyPhones
            {
                PhoneNumber = companyPhoneDto.phoneNumber
            };

            await _settingsRepository.AddCompanyPhoneNumbersAsync(phoneNumber);
            return new Response<bool>
            {
                Success = true,
                Message = "Company phone numbers added successfully",
                Data = true
            };
        }

        public async Task<Response<List<CompanyAccounts>>> AddCompanyAccountAsyn(CompanyAccountsDto companyAccountsDto)
        {
            var accounts = await _settingsRepository.GetCompanyAccountsAsync();

            foreach (var item in accounts)
            {
                if (item.BankName == companyAccountsDto.BankName && item.AccountNumber == companyAccountsDto.AccountNumber)
                    return new Response<List<CompanyAccounts>>
                    {
                        Success = false,
                        Message = "Banks already exist in the system"
                    };
            }

            var data = new CompanyAccounts
            {
                AccountName = companyAccountsDto.AccountName,
                AccountNumber = companyAccountsDto.AccountNumber,
                BankName = companyAccountsDto.BankName
            };

            await _settingsRepository.AddCompanyAccountAsync(data);

            var account = await _settingsRepository.GetCompanyAccountsAsync();
            return new Response<List<CompanyAccounts>>
            {
                Success = true,
                Message = "Account added successfully",
                Data = account
            };
        }

        public async Task<Response<List<CompanyAccounts>>> DeleteAccountsAsync(int bankId)
        {
            var result = await _settingsRepository.DeleteAccountAsync(bankId);
            if (!result)
            {
                return new Response<List<CompanyAccounts>>
                {
                    Success = false,
                    Message = "Account record doesn't exist"
                };
            }

            var accounts = await _settingsRepository.GetCompanyAccountsAsync();
            return new Response<List<CompanyAccounts>>
            {
                Success = true,
                Message = "Account Deleted successfully",
                Data = accounts
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

