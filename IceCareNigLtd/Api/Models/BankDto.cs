﻿using System;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Api.Models
{
    public class BankDto
    {
        public string BankName { get; set; }
        public string PersonType { get; set; }
        public string ExpenseType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}

