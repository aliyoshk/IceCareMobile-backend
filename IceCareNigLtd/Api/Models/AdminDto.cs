﻿using System;
namespace IceCareNigLtd.Api.Models
{
    public class AdminDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime Date { get; set; }
    }
}

