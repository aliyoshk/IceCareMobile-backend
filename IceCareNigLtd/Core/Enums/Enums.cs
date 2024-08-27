﻿using System;
namespace IceCareNigLtd.Core.Enums
{
	public class Enums
	{
        public enum BankName
        {
            UnionBank,
            WemaBank,
            UBA
        }

        public enum PersonType
        {
            Supplier,
            Customer,
            Others
        }

        public enum ModeOfPayment
        {
            Cash,
            Transfer
        }

        public enum ExpenseType
        {
            Credit,
            Debit,
        }

        public enum Channel
        {
            Mobile,
            Web,
            None
        }

        public enum ReviewAction
        {
            Approve,
            Reject
        }
    }
}

