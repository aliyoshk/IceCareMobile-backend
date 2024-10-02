using System;
namespace IceCareNigLtd.Api.Models.Request
{
	public class SupplierRequest
	{
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Date { get; set; }
        public string ModeOfPayment { get; set; }
        public List<BankInfoDto> Banks { get; set; }
        public decimal DollarRate { get; set; }
        public decimal DollarAmount { get; set; }
        public decimal Amount { get; set; }
        public string Channel { get; set; }
        public decimal Balance { get; set; }
    }

    public class BankInfoDto
    {
        public string BankName { get; set; }
        public decimal AmountTransferred { get; set; }
    }
}

