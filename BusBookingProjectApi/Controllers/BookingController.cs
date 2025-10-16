using BusBookingProjectApi.Models;
using BusBookingProjectApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BusBookingProjectApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Auth required
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepo;

        public BookingController(IBookingRepository bookingRepo)
        {
            _bookingRepo = bookingRepo;
        }

        // 🔹 Book seats (User/Admin)
        [HttpPost("book-seat")]
        public async Task<IActionResult> BookSeat([FromBody] SeatBookingRequest model)
        {
            if (model.SeatNumbers == null || !model.SeatNumbers.Any())
                return BadRequest(new { error = "Seat numbers are required" });

            var bookedSeats = await _bookingRepo.GetBookedSeatsByBusIdAsync(model.BusId);

            if (!model.IsAdmin)
            {
                var conflictSeats = model.SeatNumbers.Intersect(bookedSeats).ToList();
                if (conflictSeats.Any())
                    return BadRequest(new { error = $"Seats already booked: {string.Join(",", conflictSeats)}" });
            }

            decimal totalAmount = model.SeatNumbers.Count * 100;
            var result = await _bookingRepo.BookSeatAsync(model.UserId, model.BusId, model.SeatNumbers, totalAmount, model.IsAdmin);

            if (!result.Contains("successfully"))
                return BadRequest(new { error = result });

            return Ok(new { message = result });
        }

        // 🔹 Get booked seats for a specific bus
        [HttpGet("bus/{busId}/booked-seats")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBookedSeats(int busId)
        {
            var bookedSeats = await _bookingRepo.GetBookedSeatsByBusIdAsync(busId);
            return Ok(bookedSeats);
        }

        // 🔹 Get bookings for a user
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetBookingsByUser(int userId)
        {
            var bookings = await _bookingRepo.GetBookingsByUserIdAsync(userId);
            if (!bookings.Any())
                return NotFound(new { message = "No bookings found for this user" });
            return Ok(bookings);
        }

        // 🔹 Admin - Get all bookings
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _bookingRepo.GetAllAsync();
            return Ok(bookings);
        }

        // 🔹 Cancel booking (Admin/User)
        [HttpDelete("cancel/{id}")]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var booking = await _bookingRepo.GetByIdAsync(id);
            if (booking == null)
                return NotFound(new { error = "Booking not found" });

            // Get current user info
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (roleClaim != "Admin")
            {
                if (userIdClaim == null || int.Parse(userIdClaim) != booking.UserId)
                {
                    return Forbid("You can only cancel your own bookings");
                }
            }

            await _bookingRepo.DeleteBookingAsync(booking);
            return Ok(new { message = "Booking cancelled successfully" });
        }
    }

    // 🔹 DTO for booking request
    public class SeatBookingRequest
    {
        public int UserId { get; set; }
        public int BusId { get; set; }
        public List<int> SeatNumbers { get; set; } = new();
        public bool IsAdmin { get; set; } = false;
    }
}
