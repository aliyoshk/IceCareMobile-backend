using System;
namespace IceCareNigLtd.Api.Models.Response
{
	public class ThirdPartyPaymentResponse
	{
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string Description { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAccount { get; set; }
        public decimal Balance { get; set; }
        public string Channel { get; set; }
        public string Status { get; set; }
    }
}

