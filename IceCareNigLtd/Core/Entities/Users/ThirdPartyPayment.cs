using System;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Core.Entities.Users
{
	public class ThirdPartyPayment
	{
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Email { get; set; }
        public decimal Amount { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string Description { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAccount { get; set; }
        public Channel Channel { get; set; }
        public string Status { get; set; }
        public Category Category { get; set; }
        public string ReferenceNo { get; set; }
        public string Approver { get; set; }
    }
}

