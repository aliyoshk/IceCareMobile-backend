using System;
namespace IceCareNigLtd.Api.Models.Users
{
	public class ThirdPartyPaymentRequest
	{
		public decimal Amount { get; set; }
		public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string Description { get; set; }
        public string CustomerEmail { get; set; }
    }
}

