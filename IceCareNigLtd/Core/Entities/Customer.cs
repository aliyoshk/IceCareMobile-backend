using System;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Core.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Date { get; set; }
        public ModeOfPayment ModeOfPayment { get; set; }
        public ICollection<CustomerBankInfo> Banks { get; set; } = new List<CustomerBankInfo>();
        public decimal DollarRate { get; set; }
        public decimal DollarAmount { get; set; }
        public decimal TotalNairaAmount { get; set; }
        public decimal Balance { get; set; }
        public string AccountNumber { get; set; }
        public Channel Channel { get; set; }
        public PaymentCurrency PaymentCurrency { get; set; }
        public ICollection<CustomerPaymentReceipt> PaymentEvidence { get; set; } = new List<CustomerPaymentReceipt>();
    }

    public class CustomerBankInfo
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public decimal AmountTransferred { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }

    public class CustomerPaymentReceipt
    {
        public int Id { get; set; }
        public string Reciept { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}

