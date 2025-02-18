using System;
using System.Net.NetworkInformation;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Response;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Core.Interfaces;
using IceCareNigLtd.Infrastructure.Interfaces;
using IceCareNigLtd.Infrastructure.Repositories;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Core.Services
{
    public class BankService : IBankService
    {
        private readonly IBankRepository _bankRepository;
        private readonly ISettingsRepository _settingsRepository;

        public BankService(IBankRepository bankRepository, ISettingsRepository settingsRepository)
        {
            _bankRepository = bankRepository;
            _settingsRepository = settingsRepository;
        }

        public async Task<Response<BankDto>> AddBankAsync(BankDto bankDto)
        {
            var accounts = await _settingsRepository.GetCompanyAccountsAsync();

            if (!accounts.Any())
                return new Response<BankDto> { Success = false, Message = "Company bank(s) detail(s) is/are null" };

            //foreach(var item in accounts)
            //{
            //    if (!item.BankName.Contains(bankDto.BankName.Replace(" ", "")))
            //        return new Response<BankDto> { Success = false, Message = $"{bankDto.BankName} doesn't exist in the system" };
            //}

            var existingBank = accounts.FirstOrDefault(b => b.BankName == bankDto.BankName);

            if (existingBank == null)
                return new Response<BankDto> { Success = false, Message = $"{bankDto.BankName} doesn't exist in the system" };

            if (bankDto.EntityName == "")
                return new Response<BankDto> { Success = false, Message = $"Fill in the name" };

            if (bankDto.Amount <= 0)
                return new Response<BankDto> { Success = false, Message = $"The amount should be greather than 0" };


            var bank = new Bank
            {
                EntityName = bankDto.EntityName,
                BankName = bankDto.BankName.ToString(),
                Date = DateTime.UtcNow,
                PersonType = Enum.Parse<PersonType>(bankDto.PersonType),
                ExpenseType = Enum.Parse<CreditType>(bankDto.ExpenseType),
                Amount = bankDto.Amount,
            };

            await _bankRepository.AddBankAsync(bank);

            return new Response<BankDto> { Success = true, Message = "Bank record added successfully", Data = bankDto };
        }

        public async Task<Response<List<BankDto>>> GetBanksAsync()
        {
            var banks = await _bankRepository.GetBanksAsync();
            var bankDtos = banks.Select(b => new BankDto
            {
                Id = b.Id,
                EntityName = b.EntityName,
                BankName = b.BankName.ToString(),
                Date = DateTime.Now,
                PersonType = b.PersonType.ToString(),
                ExpenseType = b.ExpenseType.ToString(),
                Amount = b.Amount
            }).ToList();

            return new Response<List<BankDto>> { Success = true, Message = "Banks retrieved successfully", Data = bankDtos };
        }


        public async Task<Response<List<BankDto>>> GetBankRecordByNameAsync(string bankName)
        {
            var banks = await _bankRepository.GetBankRecordByNameAsync(bankName);

            var bankDtos = banks.Select(b => new BankDto
            {
                Id = b.Id,
                EntityName = b.EntityName,
                BankName = b.BankName.ToString(),
                Date = DateTime.UtcNow,
                PersonType = b.PersonType.ToString(),
                ExpenseType = b.ExpenseType.ToString(),
                Amount = b.Amount
            }).ToList();

            return new Response<List<BankDto>> { Success = true, Message = "Banks retrieved successfully", Data = bankDtos };
        }

        public async Task<Response<object>> DeleteBankAsync(int bankId)
        {
            await _bankRepository.DeleteBankAsync(bankId);
            return new Response<object> { Success = true, Message = "Record deleted successfully" };
        }
    }
}

