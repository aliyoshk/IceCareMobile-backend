using System;
namespace IceCareNigLtd.Api.Models.Response
{
	public class ThirdPartyPaymentResponse
	{
        public int Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string Description { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAccount { get; set; }
        public string Channel { get; set; }
        public string Status { get; set; }
        public string TransferReference { get; set; }
        public string Category { get; set; }
        public string PhoneNumber { get; set; }
    }
}

