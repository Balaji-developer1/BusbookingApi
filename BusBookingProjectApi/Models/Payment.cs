using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingProjectApi.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Booking ID is required")]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(1, 1000, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment status is required")]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Possible values: Pending, Success, Failed

        [StringLength(100)]
        public string? TransactionId { get; set; } // Unique transaction ID after payment

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property to Booking
        [ForeignKey("BookingId")]
        public Booking? Booking { get; set; }
    }
}
