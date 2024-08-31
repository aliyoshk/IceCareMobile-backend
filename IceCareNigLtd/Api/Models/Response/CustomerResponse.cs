using System;
using IceCareNigLtd.Api.Models.Request;

namespace IceCareNigLtd.Api.Models.Response
{
	public class CustomerResponse
	{
        public List<CustomerResponseDto> Customers { get; set; }
        public int TotalCustomers { get; set; }
        public decimal TotalDollarAmount { get; set; }
        public decimal TotalNairaAmount { get; set; }
    }

    public class CustomerResponseDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Date { get; set; }
        public string ModeOfPayment { get; set; }
        public List<BankInfoDto> Banks { get; set; }
        public decimal DollarRate { get; set; }
        public decimal DollarAmount { get; set; }
        public decimal Balance { get; set; }
        public decimal Amount { get; set; }
        public string PaymentCurrency { get; set; }
        public List<ReceiptDto> PaymentEvidence { get; set; }
        public string AccountNumber { get; set; }
    }

    public class ReceiptDto
    {
        public string Receipt { get; set; }
    }
}

