using System;
namespace IceCareNigLtd.Core.Entities
{
	public class Settings
	{
        public int Id { get; set; }
        public decimal DollarRate { get; set; }
        public string? CompanyPhoneNumbers { get; set; }
        public List<CompanyAccounts> CompanyAccounts { get; set; } = new List<CompanyAccounts>();
    }
}

