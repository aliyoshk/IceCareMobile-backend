using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Core.Interfaces;
using IceCareNigLtd.Infrastructure.Interfaces;

namespace IceCareNigLtd.Core.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<Response<PaymentDto>> AddPaymentAsync(PaymentDto paymentDto)
        {
            var payment = new Payment
            {
                CustomerName = paymentDto.CustomerName,
                Date = DateTime.UtcNow,
                DollarAmount = paymentDto.DollarAmount
            };

            await _paymentRepository.AddPaymentAsync(payment);

            return new Response<PaymentDto> { Success = true, Message = "Payment added successfully", Data = paymentDto };
        }

        public async Task<Response<List<PaymentDto>>> GetPaymentsAsync()
        {
            var payments = await _paymentRepository.GetPaymentsAsync();

            if (!payments.Any())
            {
                return new Response<List<PaymentDto>>
                {
                    Success = false,
                    Message = "Payment record is empty",
                };
            }

            var paymentDtos = payments.Select(p => new PaymentDto
            {
                Id = p.Id,
                CustomerName = p.CustomerName,
                Date = p.Date,
                DollarAmount = p.DollarAmount
            }).ToList();

            return new Response<List<PaymentDto>> { Success = true, Message = "Payments retrieved successfully", Data = paymentDtos };
        }
    }
}

