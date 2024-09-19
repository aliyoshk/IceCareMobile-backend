using System;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Core.Entities
{
    public class Bank
    {
        public int Id { get; set; }
        public string EntityName { get; set; }
        public BankName BankName { get; set; }
        public DateTime Date { get; set; }
        public PersonType PersonType { get; set; } // Credit, Debit
        public CreditType ExpenseType { get; set; }
        public decimal Amount { get; set; }
    }
}

