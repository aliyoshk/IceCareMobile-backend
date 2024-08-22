using System;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Core.Entities
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Date { get; set; }
        public ModeOfPayment ModeOfPayment { get; set; } // Cash, Transfer, Others
        public ICollection<BankInfo> Banks { get; set; } = new List<BankInfo>(); // List of Banks
        public decimal DollarRate { get; set; }
        public decimal DollarAmount { get; set; }
        public decimal TotalDollarAmount { get; set; }
        public decimal TotalNairaAmount { get; set; }
    }

    public class BankInfo
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public decimal AmountTransferred { get; set; }
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
    }
}

