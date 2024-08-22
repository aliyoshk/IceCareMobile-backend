using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Core.Interfaces;
using IceCareNigLtd.Infrastructure.Interfaces;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Core.Services
{
    public class BankService : IBankService
    {
        private readonly IBankRepository _bankRepository;

        public BankService(IBankRepository bankRepository)
        {
            _bankRepository = bankRepository;
        }

        public async Task<Response<BankDto>> AddBankAsync(BankDto bankDto)
        {
            var bank = new Bank
            {
                BankName = Enum.Parse<BankName>(bankDto.BankName.ToString()),
                Date = bankDto.Date,
                PersonType = Enum.Parse<PersonType>(bankDto.PersonType.ToString()),
                ExpenseType = Enum.Parse<ExpenseType>(bankDto.PersonType.ToString()),
                Amount = bankDto.Amount
            };

            await _bankRepository.AddBankAsync(bank);

            return new Response<BankDto> { Success = true, Message = "Bank record added successfully", Data = bankDto };
        }

        public async Task<Response<List<BankDto>>> GetBanksAsync()
        {
            var banks = await _bankRepository.GetBanksAsync();
            var bankDtos = banks.Select(b => new BankDto
            {
                BankName = b.BankName.ToString(),
                Date = b.Date,
                PersonType = b.PersonType.ToString(),
                ExpenseType = b.ExpenseType.ToString(),
                Amount = b.Amount
            }).ToList();

            return new Response<List<BankDto>> { Success = true, Message = "Banks retrieved successfully", Data = bankDtos };
        }
    }
}

