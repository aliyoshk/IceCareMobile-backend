using System;
using IceCareNigLtd.Api.Models.Request;

namespace IceCareNigLtd.Api.Models.Response
{
	public class SupplierResponse
	{
        public List<SupplierRequest> Suppliers { get; set; }
        public int TotalSuppliers { get; set; }
        public decimal TotalDollarAmount { get; set; }
        public decimal TotalNairaAmount { get; set; }
    }
}

