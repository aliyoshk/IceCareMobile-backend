using System;
namespace IceCareNigLtd.Api.Models
{
    public class CustomerDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Date { get; set; }
        public string ModeOfPayment { get; set; }
        public List<BankInfoDto> Banks { get; set; }
        public decimal DollarRate { get; set; }
        public decimal DollarAmount { get; set; }
        public decimal TotalDollarAmount { get; set; }
        public decimal TotalNairaAmount { get; set; }
    }

    public class CustomersResponseDto
    {
        public List<CustomerDto> Customers { get; set; }
        public int TotalCustomers { get; set; }
    }
}

