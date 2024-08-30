using System;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Core.Entities.Users
{
	public class Transfer
	{
        public int Id { get; set; }
        public decimal DollarAmount { get; set; }
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAccount { get; set; }
        public decimal Balance { get; set; }
        public Channel Channel { get; set; }
        public string Email { get; set; }
        public decimal DollarRate { get; set; }
        public string TransferReference { get; set; }
        public string Status { get; set; }
        public string Approver { get; set; }
        public Category Category { get; set; }

        public List<TransferBank> BankDetails { get; set; }
        public List<EvidenceOfTransfer> TransferEvidence { get; set; }
    }

    public class TransferBank
    {
        public int Id { get; set; }
        public BankName BankName { get; set; }
        public decimal TransferredAmount { get; set; }

        public int TransferId { get; set; }
        public Transfer Transfer { get; set; }
    }

    public class EvidenceOfTransfer
    {
        public int Id { get; set; }
        public string Receipts { get; set; }

        public int TransferId { get; set; }
        public Transfer Transfer { get; set; }
    }
}

