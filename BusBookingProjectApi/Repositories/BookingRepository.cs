using BusBookingProjectApi.Data;
using BusBookingProjectApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusBookingProjectApi.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddBookingAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Bus)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Booking>> GetBookingsByUserIdAsync(int userId)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.Bus)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Bus)
                .ToListAsync();
        }

        // 🔹 Updated BookSeatAsync
        public async Task<string> BookSeatAsync(int userId, int busId, List<int> seatNumbers, decimal totalAmount, bool isAdmin = false)
        {
            // All bookings for this bus
            var allBookings = await _context.Bookings
                .Where(b => b.BusId == busId)
                .ToListAsync();

            // Already booked seats
            var alreadyBooked = allBookings
                .SelectMany(b => b.SeatNumbers ?? new List<int>())
                .Intersect(seatNumbers)
                .ToList();

            if (alreadyBooked.Any() && !isAdmin)
                return $"Seats already booked: {string.Join(",", alreadyBooked)}";

            if (isAdmin)
            {
                foreach (var booking in allBookings)
                {
                    booking.SeatNumbers = booking.SeatNumbers.Except(seatNumbers).ToList();
                    booking.Seats = booking.SeatNumbers.Count;
                }
            }

            var bookingEntry = new Booking
            {
                UserId = userId,
                BusId = busId,
                Seats = seatNumbers.Count,
                SeatNumbers = seatNumbers,
                TotalAmount = totalAmount
            };

            await _context.Bookings.AddAsync(bookingEntry);
            await _context.SaveChangesAsync();

            return "Seat booked successfully";
        }

        // 🔹 Get booked seats by bus
        public async Task<List<int>> GetBookedSeatsByBusIdAsync(int busId)
        {
            var allBookings = await _context.Bookings
                .Where(b => b.BusId == busId)
                .ToListAsync();

            return allBookings
                .SelectMany(b => b.SeatNumbers ?? new List<int>())
                .ToList();
        }

        public async Task DeleteBookingAsync(Booking booking)
        {
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }
    }
}
