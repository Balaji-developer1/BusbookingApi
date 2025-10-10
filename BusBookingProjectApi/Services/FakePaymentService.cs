using System;
using System.Threading.Tasks;
using BusBookingProjectApi.Models;

namespace BusBookingProjectApi.Services
{
    public class FakePaymentService : IFakePaymentService
    {
        public Task<Payment> ProcessPaymentAsync(Payment payment)
        {
            payment.Status = "Success";
            payment.TransactionId = Guid.NewGuid().ToString();
            return Task.FromResult(payment);
        }
    }
}
