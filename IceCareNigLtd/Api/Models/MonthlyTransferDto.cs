using System;
namespace IceCareNigLtd.Api.Models
{
	public class MonthlyTransferDto
	{
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalAmount { get; set; }
    }
}

