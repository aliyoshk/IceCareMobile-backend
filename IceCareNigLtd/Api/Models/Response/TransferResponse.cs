using System;
using IceCareNigLtd.Api.Models.Request;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Api.Models.Response
{
	public class TransferResponse
    {
        public int Id { get; set; }
        public decimal DollarAmount { get; set; }
        public string Description { get; set; }
        public List<BankDetails> BankDetails { get; set; }
        public List<TransferEvidence> TransferEvidence { get; set; }
        public DateTime TransactionDate { get; set; }
        public string CustomerEmail { get; set; }
        public decimal DollarRate { get; set; }
        public string Status { get; set; } 
        public Category Category { get; set; }
        public string ApproverName { get; set; }
    }
}

