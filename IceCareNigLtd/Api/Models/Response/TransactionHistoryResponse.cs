using System;
using IceCareNigLtd.Core.Entities.Users;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Api.Models.Response
{
	public class TransactionHistoryResponse
	{
        public DateTime TransactionDate { get; set; }
        public string TotalAmount { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public List<AccountDetails> AccountDetails { get; set; }
	}

    public class AccountDetails
    {
        public string Amount { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
    }
}

