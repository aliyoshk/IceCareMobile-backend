using System;
using IceCareNigLtd.Api.Models;
using IceCareNigLtd.Api.Models.Request;
using IceCareNigLtd.Core.Entities;
using IceCareNigLtd.Core.Interfaces;
using IceCareNigLtd.Infrastructure.Interfaces;
using IceCareNigLtd.Infrastructure.Repositories;
using static IceCareNigLtd.Core.Enums.Enums;

namespace IceCareNigLtd.Core.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ISupplierRepository _supplierRepository;

        public PaymentService(IPaymentRepository paymentRepository, ISupplierRepository supplierRepository)
        {
            _paymentRepository = paymentRepository;
            _supplierRepository = supplierRepository;
        }

        public async Task<Response<PaymentDto>> AddPaymentAsync(PaymentDto paymentDto)
        {
            var availableDollarAmount = await _supplierRepository.GetTotalDollarAmountAsync();

            if (paymentDto.DollarAmount > availableDollarAmount)
                return new Response<PaymentDto> { Success = false, Message = "Insufficient dollar to complete request", Data = paymentDto };

            var payment = new Payment
            {
                CustomerName = paymentDto.CustomerName,
                Date = DateTime.UtcNow,
                DollarAmount = paymentDto.DollarAmount,
                Balance = paymentDto.Balance,
                Deposit = paymentDto.Deposit
            };

            await _supplierRepository.SubtractDollarAmountAsync(paymentDto.DollarAmount);
            await _paymentRepository.AddPaymentAsync(payment);

            return new Response<PaymentDto> { Success = true, Message = "Payment added successfully", Data = paymentDto };
        }

        public async Task<Response<object>> DeletePaymentAsync(int paymentId)
        {
            await _paymentRepository.DeletePaymentAsync(paymentId);
            return new Response<object> { Success = true, Message = "Record deleted successfully" };
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
                DollarAmount = p.DollarAmount,
                Balance = p.Balance,
                Deposit = p.Deposit
            }).ToList();

            return new Response<List<PaymentDto>> { Success = true, Message = "Payments retrieved successfully", Data = paymentDtos };
        }
    }
}

