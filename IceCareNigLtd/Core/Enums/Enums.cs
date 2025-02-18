using System;
namespace IceCareNigLtd.Core.Enums
{
	public class Enums
	{
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

        public enum CreditType
        {
            Credit,
            Debit,
        }

        public enum Channel
        {
            Mobile,
            Web,
            WalkIn
        }

        public enum ReviewAction
        {
            Approve,
            Reject
        }

        public enum PaymentCurrency
        {
            Naira,
            Dollar
        }

        public enum Category
        {
            SingleBankPayment,
            MultipleBanksPayment,
            AccountBalancePayment,
            ThirdPartyPayment,
            AccountTopUp
        }
    }
}

