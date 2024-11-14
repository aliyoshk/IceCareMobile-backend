using System;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Core.Entities.Users
{
	public class AccountPayment
	{
        public int Id { get; set; }
        public decimal NairaAmount { get; set; }
        public decimal DollarAmount { get; set; }
        public string Description { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAccount { get; set; }
        public decimal Balance { get; set; }
        public Channel Channel { get; set; }
    }
}

