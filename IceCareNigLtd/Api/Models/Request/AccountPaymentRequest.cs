using System;
namespace IceCareNigLtd.Api.Models.Request
{
	public class AccountPaymentRequest
    {
		public decimal NairaAmount { get; set; }
        public decimal DollarAmount { get; set; }
		public string Description { get; set; }
		public string CustomerEmail { get; set; }
    }
}

