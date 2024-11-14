using System;
namespace IceCareNigLtd.Api.Models.Request
{
	public class ConfirmationRequest
	{
		public int Id { get; set; }
		public string Email { get; set; }
		public bool Confirmed { get; set; }
	}
}

