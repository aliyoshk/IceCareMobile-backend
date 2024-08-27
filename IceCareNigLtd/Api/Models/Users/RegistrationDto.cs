using System;
namespace IceCareNigLtd.Api.Models.Users
{
	public class RegistrationDto
	{
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
    }
}

