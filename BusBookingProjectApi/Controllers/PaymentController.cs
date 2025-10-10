using BusBookingProjectApi.Models;
using BusBookingProjectApi.Repositories;
using BusBookingProjectApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BusBookingProjectApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly IBookingRepository _bookingRepo;
        private readonly IFakePaymentService _fakePayment;

        public PaymentController(
            IPaymentRepository paymentRepo,
            IBookingRepository bookingRepo,
            IFakePaymentService fakePayment)
        {
            _paymentRepo = paymentRepo;
            _bookingRepo = bookingRepo;
            _fakePayment = fakePayment;
        }

        // 🔹 Make payment for a booking (fake)
        [HttpPost("process/{bookingId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> ProcessPayment(int bookingId)
        {
            // Check booking exists
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null) return NotFound("Booking not found");

            // Check if payment already exists
            var existingPayment = await _paymentRepo.GetByBookingIdAsync(bookingId);
            if (existingPayment != null) return BadRequest("Payment already exists for this booking");

            // Create Payment
            var payment = new Payment
            {
                BookingId = bookingId,
                Amount = booking.TotalAmount,
                Status = "Pending"
            };

            // Simulate fake payment
            payment = await _fakePayment.ProcessPaymentAsync(payment);

            // Save payment
            await _paymentRepo.AddPaymentAsync(payment);

            return Ok(new
            {
                paymentId = payment.Id,
                bookingId = payment.BookingId,
                transactionId = payment.TransactionId,
                status = payment.Status
            });
        }

        // 🔹 Get payment info by booking
        [HttpGet("booking/{bookingId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetPayment(int bookingId)
        {
            var payment = await _paymentRepo.GetByBookingIdAsync(bookingId);
            if (payment == null) return NotFound("Payment not found");
            return Ok(payment);
        }
    }
}
