using System;
using IceCareNigLtd.Api.Models.Users;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Api.Models
{
	public class ChangeUserStatusRequest
	{
        public int UserId { get; set; }
        public ReviewAction Action { get; set; }
    }
}

