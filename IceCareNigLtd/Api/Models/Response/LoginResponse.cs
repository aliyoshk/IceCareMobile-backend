﻿using System;
using IceCareNigLtd.Core.Entities;

namespace IceCareNigLtd.Api.Models.Response
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public string AccountNumber { get; set; }
        public UserAccount UserAccount { get; set; }
    }

    public class UserAccount
    {
        public decimal DollarRate { get; set; }
        public string NairaBalance { get; set; }
        public string DollarBalance { get; set; }
        public List<CompanyPhones> CompanyNumber { get; set; }
        public List<CompanyAccounts> CompanyAccounts { get; set; } = new List<CompanyAccounts>();
    }
}

