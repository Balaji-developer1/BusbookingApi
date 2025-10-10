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
        Task UpdateBookingAsync(Booking booking);

        // 🔹 New methods for seat-level booking
        Task<string> BookSeatAsync(int userId, int busId, int seatNumber);
        Task<List<BusSeat>> GetSeatsByBusIdAsync(int busId);
    }
}
