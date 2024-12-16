using System;
using IceCareNigLtd.Api.Models.Request;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Api.Models.Response
{
	public class TransferResponse
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public decimal DollarAmount { get; set; }
        public string Description { get; set; }
        public List<TransferDetails> BankDetails { get; set; }
        public List<TransferEvidence> TransferEvidence { get; set; }
        public DateTime TransactionDate { get; set; }
        public string CustomerEmail { get; set; }
        public decimal DollarRate { get; set; }
        public string Status { get; set; } 
        public string Category { get; set; }
        public string ApproverName { get; set; }
        public decimal Balance { get; set; }
        public string TransferReference { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class TransferDetails
    {
        public string BankName { get; set; }
        public decimal AmountTransferred { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
    }
}

