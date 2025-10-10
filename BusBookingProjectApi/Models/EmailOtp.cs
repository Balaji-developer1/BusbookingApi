using System;
using System.ComponentModel.DataAnnotations;

namespace BusBookingProjectApi.Models
{
    public class EmailOtp
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "OTP hash is required")]
        public string OtpHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "Salt is required")]
        public string Salt { get; set; } = string.Empty;

        public DateTime ExpiresAt { get; set; }

        public int Attempts { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
