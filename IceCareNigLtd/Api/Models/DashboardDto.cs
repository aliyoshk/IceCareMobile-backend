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
        public List<CompanyPhones> CompanyPhoneNumbers { get; set; }
        public string AdminRole { get; set; }
        public bool ShowAdminPanel { get; set; }
        public decimal TotalMonthlyNairaTransfer { get; set; }
        public decimal TotalMonthlyDollarSpent { get; set; }
        public decimal TotalDailyNairaTransfer { get; set; }
        public List<CompanyAccounts> CompanyAccounts { get; set; } = new List<CompanyAccounts>();
        public List<PendingTransfer> PendingTransfer { get; set; } = new List<PendingTransfer>();
        public List<PendingRegistration> PendingRegistration { get; set; } = new List<PendingRegistration>();
        public List<MonthlyTransferDto> MonthlyTransfers { get; set; } = new List<MonthlyTransferDto>();
    }

    public class PendingTransfer
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public decimal amount { get; set; }
    }

    public class PendingRegistration
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
    }
}

