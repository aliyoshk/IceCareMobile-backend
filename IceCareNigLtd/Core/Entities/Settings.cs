using System;
namespace IceCareNigLtd.Core.Entities
{
	public class Settings
	{
        public int Id { get; set; }
        public decimal DollarRate { get; set; }
        public List<CompanyPhones> CompanyPhoneNumbers { get; set; } = new List<CompanyPhones>();
        public List<CompanyAccounts> CompanyAccounts { get; set; } = new List<CompanyAccounts>();
    }
}

