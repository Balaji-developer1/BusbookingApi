using BusBookingProjectApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusBookingProjectApi.Repositories
{
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(int id);
        Task<List<Booking>> GetBookingsByUserIdAsync(int userId);
        Task AddBookingAsync(Booking booking);
        Task<List<Booking>> GetAllAsync();
        Task DeleteBookingAsync(Booking booking);
        Task<string> BookSeatAsync(int userId, int busId, List<int> seatNumbers, decimal totalAmount, bool isAdmin = false);
        Task<List<int>> GetBookedSeatsByBusIdAsync(int busId);
    }
}
