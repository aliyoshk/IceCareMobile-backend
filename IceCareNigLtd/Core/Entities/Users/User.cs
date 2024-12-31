using System;
namespace IceCareNigLtd.Core.Entities.Users
{
	public class User
	{
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime Date { get; set; }
        public string AccountNumber { get; set; }
        public decimal BalanceNaira { get; set; }
        public decimal BalanceDollar { get; set; }
    }
}

