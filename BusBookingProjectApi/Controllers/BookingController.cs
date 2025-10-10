using BusBookingProjectApi.Models;
using BusBookingProjectApi.Repositories;
using BusBookingProjectApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusBookingProjectApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IBusRepository _busRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly IFakePaymentService _paymentService;

        public BookingController(
            IBookingRepository bookingRepo,
            IBusRepository busRepo,
            IPaymentRepository paymentRepo,
            IFakePaymentService paymentService)
        {
            _bookingRepo = bookingRepo;
            _busRepo = busRepo;
            _paymentRepo = paymentRepo;
            _paymentService = paymentService;
        }

        // 🔹 Book a specific seat
        [HttpPost("book-seat")]
        public async Task<IActionResult> BookSeat([FromBody] SeatBookingRequest model)
        {
            var bus = await _busRepo.GetByIdAsync(model.BusId);
            if (bus == null) return NotFound(new { error = "Bus not found" });

            // Check seat availability
            var result = await _bookingRepo.BookSeatAsync(model.UserId, model.BusId, model.SeatNumber);
            if (result != "Seat booked successfully")
                return BadRequest(new { error = result });

            // Get updated bus info
            bus = await _busRepo.GetByIdAsync(model.BusId);

            // Create payment for this seat
            var payment = new Payment
            {
                BookingId = bus.Id,
                Amount = bus.Fare,
                Status = "Pending"
            };

            payment = await _paymentService.ProcessPaymentAsync(payment);
            await _paymentRepo.AddPaymentAsync(payment);

            return Ok(new
            {
                message = "Seat booked successfully",
                busId = bus.Id,
                seatNumber = model.SeatNumber,
                paymentId = payment.Id,
                status = payment.Status
            });
        }

        // 🔹 Get all seats for a bus (red/green status)
        [HttpGet("bus/{busId}/seats")]
        public async Task<IActionResult> GetSeats(int busId)
        {
            var seats = await _bookingRepo.GetSeatsByBusIdAsync(busId);
            return Ok(seats);
        }

        // 🔹 Get bookings by user
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetBookings(int userId)
        {
            var bookings = await _bookingRepo.GetBookingsByUserIdAsync(userId);
            return Ok(bookings);
        }
    }

    // 🔹 Request DTOs
    public class SeatBookingRequest
    {
        public int UserId { get; set; }
        public int BusId { get; set; }
        public int SeatNumber { get; set; }  // seat user wants to book
    }
}
