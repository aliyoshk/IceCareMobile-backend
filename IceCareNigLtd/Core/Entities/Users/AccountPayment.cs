using System;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Core.Entities.Users
{
	public class AccountPayment
	{
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAccount { get; set; }
        public decimal DollarRate { get; set; }
        public decimal Amount { get; set; }
        public decimal DollarAmount { get; set; }
        public Channel Channel { get; set; }
        public PaymentCurrency Currency { get; set; }
        public Category Category { get; set; }
        public string ReferenceNo { get; set; }
        public string Status { get; set; }
        public string Reviewer { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}

