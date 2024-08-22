using System;
namespace IceCareNigLtd.Api.Models.Network
{
	public class ErrorResponse : Response<object>
    {
        public new bool Success { get; set; } = false;
        public new string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}

