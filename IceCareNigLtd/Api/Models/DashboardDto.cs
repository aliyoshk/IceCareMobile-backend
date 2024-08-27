using System;
using IceCareNigLtd.Core.Entities;

namespace IceCareNigLtd.Api.Models
{
	public class DashboardDto
	{
        public string AdminName { get; set; }
        public int NumberOfSuppliers { get; set; }
        public int NumberOfCustomers { get; set; }
        public decimal TotalTransferredAmount { get; set; }
        public decimal AvailableDollarAmount { get; set; }
        public decimal DollarRate { get; set; }
        public string CompanyPhoneNumbers { get; set; }
        public string AdminRole { get; set; }
        public bool ShowAdminPanel { get; set; }
        public List<CompanyAccounts> CompanyAccounts = new List<CompanyAccounts>();
    }
}

