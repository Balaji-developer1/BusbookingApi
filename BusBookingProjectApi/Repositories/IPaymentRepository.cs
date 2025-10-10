
using BusBookingProjectApi.Models;
using System.Threading.Tasks;

namespace BusBookingProjectApi.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment> AddPaymentAsync(Payment payment);
        Task<Payment?> GetByBookingIdAsync(int bookingId);
        Task UpdatePaymentAsync(Payment payment);
    }
}
