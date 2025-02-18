using System;
using IceCareNigLtd.Core.Entities.Users;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Api.Models.Request
{
	public class AccoutTopUpRequest
	{
        public string Currency { get; set; }
        public string Description { get; set; }
        public string AccountNo { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<BankDetails> BankDetails { get; set; }
        public List<TransferEvidence> TransferEvidence { get; set; }
    }
}

