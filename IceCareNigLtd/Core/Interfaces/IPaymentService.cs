using System;
using IceCareNigLtd.Api.Models;

namespace IceCareNigLtd.Core.Interfaces
{
	public interface IPaymentService
	{
        Task<Response<PaymentDto>> AddPaymentAsync(PaymentDto paymentDto);
        Task<Response<List<PaymentDto>>> GetPaymentsAsync();
    }
}

