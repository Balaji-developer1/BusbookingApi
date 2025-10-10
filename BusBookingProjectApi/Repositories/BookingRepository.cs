using BusBookingProjectApi.Data;
using BusBookingProjectApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
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
                .Include(b => b.Bus)
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        public async Task AddBookingAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBookingAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }

        // 🔹 New: Seat-level booking
        public async Task<string> BookSeatAsync(int userId, int busId, int seatNumber)
        {
            // 1️⃣ Check bus exists
            var bus = await _context.Buses.FindAsync(busId);
            if (bus == null) return "Bus not found";

            // 2️⃣ Check seat exists
            var seat = await _context.BusSeats
                .FirstOrDefaultAsync(s => s.BusId == busId && s.SeatNumber == seatNumber);
            if (seat == null) return "Seat not found";

            // 3️⃣ Check if already booked
            if (seat.IsBooked) return "Seat already booked";

            // 4️⃣ Mark seat as booked
            seat.IsBooked = true;
            seat.BookedByUserId = userId;
            _context.BusSeats.Update(seat);

            // 5️⃣ Create booking record
            var booking = new Booking
            {
                BusId = busId,
                UserId = userId,
                Seats = 1,
                TotalAmount = bus.Fare
            };
            _context.Bookings.Add(booking);

            await _context.SaveChangesAsync();
            return "Seat booked successfully";
        }

        // 🔹 New: Get all seats for a bus
        public async Task<List<BusSeat>> GetSeatsByBusIdAsync(int busId)
        {
            return await _context.BusSeats
                .Where(s => s.BusId == busId)
                .ToListAsync();
        }
    }
}
