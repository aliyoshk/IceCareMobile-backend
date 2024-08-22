using System;
using IceCareNigLtd.Core.Entities;

namespace IceCareNigLtd.Infrastructure.Interfaces
{
	public interface IPaymentRepository
	{
        Task AddPaymentAsync(Payment payment);
        Task<List<Payment>> GetPaymentsAsync();
    }
}

