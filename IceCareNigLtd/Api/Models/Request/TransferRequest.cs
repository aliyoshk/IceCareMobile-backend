﻿using System;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Api.Models.Request
{
	public class TransferRequest
	{
        public decimal DollarAmount { get; set; }
		public string Description { get; set; }
        public List<BankDetails> BankDetails { get; set; } 
        public List<TransferEvidence> TransferEvidence { get; set; }
        public DateTime TransactionDate { get; set; }
        public string CustomerEmail { get; set; }
    }

    public class BankDetails
    {
        public BankName BankName { get; set; }
        public decimal TransferredAmount { get; set; }
    }

    public class TransferEvidence
    {
        public string Receipts { get; set; }
    }
}
