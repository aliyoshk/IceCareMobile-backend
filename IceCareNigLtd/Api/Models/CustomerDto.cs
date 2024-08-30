﻿using System;
using IceCareNigLtd.Api.Models.Request;

namespace IceCareNigLtd.Api.Models
{
    public class CustomerDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Date { get; set; }
        public string ModeOfPayment { get; set; }
        public List<BankInfoDto> Banks { get; set; }
        public decimal DollarRate { get; set; }
        public decimal DollarAmount { get; set; }
        public decimal Balance { get; set; }
        public decimal Amount { get; set; }
        public string PaymentCurrency { get; set; }
        public string PaymentEvidence { get; set; }
    }
}

