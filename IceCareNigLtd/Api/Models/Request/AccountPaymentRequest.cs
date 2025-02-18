using System;
namespace IceCareNigLtd.Api.Models.Request
{
	public class AccountPaymentRequest
    {
		public decimal Amount { get; set; }
		public string Description { get; set; }
		public string CustomerEmail { get; set; }
    }
}

