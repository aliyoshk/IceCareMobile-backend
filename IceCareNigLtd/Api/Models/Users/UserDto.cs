using System;
namespace IceCareNigLtd.Api.Models.Users
{
	public class UserDto
	{
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public string AccountNumber { get; set; }
        public string ReviewedBy { get; set; }
    }
}

