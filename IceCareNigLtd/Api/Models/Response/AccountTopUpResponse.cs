using System;
using IceCareNigLtd.Api.Models.Request;

namespace IceCareNigLtd.Api.Models.Response
{
	public class AccountTopUpResponse
	{
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<BankDetails> BankDetails { get; set; }
        public List<TransferEvidence> TransferEvidence { get; set; }
    }
}

