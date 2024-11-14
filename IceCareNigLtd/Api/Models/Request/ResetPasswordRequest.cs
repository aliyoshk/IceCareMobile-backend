using System;
namespace IceCareNigLtd.Api.Models.Request
{
	public class ResetPasswordRequest
	{
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string NewPassword { get; set; }
    }
}

