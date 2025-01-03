using System;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Core.Entities.Users
{
	public class AccountTopUp
	{
        public int Id { get; set; }
        public Category Category { get; set; }
        public PaymentCurrency Currency { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string AccountNo { get; set; }
        public string Reference { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Approver { get; set; }
        public DateTime TransactionDate { get; set; }
        public List<TransferDetail> TransferDetails { get; set; }
        public List<TopUpEvidence> TransferEvidence { get; set; }
    }

    public class TransferDetail
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public decimal TransferredAmount { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }

        public int AccountTopUpId { get; set; }
        public AccountTopUp AccountTopUp { get; set; }
    }

    public class TopUpEvidence
    {
        public int Id { get; set; }
        public string Receipts { get; set; }

        public int AccountTopUpId { get; set; }
        public AccountTopUp AccountTopUp { get; set; }
    }
}

