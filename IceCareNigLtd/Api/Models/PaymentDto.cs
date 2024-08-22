using System;
namespace IceCareNigLtd.Api.Models
{
	public class PaymentDto
	{
        public string CustomerName { get; set; }
        public DateTime Date { get; set; }
        public string ModeOfPayment { get; set; }
        public decimal DollarAmount { get; set; }
    }
}

