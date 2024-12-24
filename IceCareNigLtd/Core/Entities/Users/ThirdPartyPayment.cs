using System;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Core.Entities.Users
{
	public class ThirdPartyPayment
	{
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string Description { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAccount { get; set; }
        public decimal BalanceNaira { get; set; }
        public decimal BalanceDollar { get; set; }
        public Channel Channel { get; set; }
        public string Status { get; set; }
    }
}

