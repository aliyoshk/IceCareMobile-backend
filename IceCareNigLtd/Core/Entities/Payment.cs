using System;
namespace IceCareNigLtd.Core.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public DateTime Date { get; set; }
        public string ModeOfPayment { get; set; }
        public decimal DollarAmount { get; set; }
    }
}

