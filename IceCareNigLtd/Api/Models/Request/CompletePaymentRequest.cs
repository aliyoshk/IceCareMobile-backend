using System;
namespace IceCareNigLtd.Api.Models.Request
{
	public class CompletePaymentRequest
	{
		public int CustomerId { get; set; }
        public decimal DollarAmount { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Charges { get; set; }
    }
}

