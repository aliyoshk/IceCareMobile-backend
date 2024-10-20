﻿using System;
namespace IceCareNigLtd.Api.Models
{
	public class PaymentDto
	{
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public DateTime Date { get; set; }
        public decimal DollarAmount { get; set; }
        public decimal Balance { get; set; }
        public decimal Deposit { get; set; }
    }
}

