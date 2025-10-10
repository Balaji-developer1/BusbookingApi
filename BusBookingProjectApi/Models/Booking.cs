using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingProjectApi.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Bus ID is required")]
        public int BusId { get; set; }

        [Required(ErrorMessage = "Seats count is required")]
        [Range(1, 20, ErrorMessage = "Seats must be 1–20")]
        public int Seats { get; set; }

        [Required(ErrorMessage = "Total amount is required")]
        public decimal TotalAmount { get; set; }

        public string? PaymentTransactionId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [ForeignKey("BusId")]
        public Bus? Bus { get; set; }



        public List<int>? SeatNumbers { get; set; } // store seat numbers booked in this record
        

    }
}
