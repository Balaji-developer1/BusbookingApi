using System.Threading.Tasks;
using BusBookingProjectApi.Models;

namespace BusBookingProjectApi.Services
{
    public interface IFakePaymentService
    {
        Task<Payment> ProcessPaymentAsync(Payment payment);
    }
}
