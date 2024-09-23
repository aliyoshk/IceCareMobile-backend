using System;
namespace IceCareNigLtd.Api.Models.Response
{
	public class AdminResponseDto
	{
        public string AdminName { get; set; }
        public bool ShowAdminPanel { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
    }
}

