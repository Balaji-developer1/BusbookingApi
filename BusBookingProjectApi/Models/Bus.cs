using System;
using System.ComponentModel.DataAnnotations;

namespace BusBookingProjectApi.Models
{
    public class Bus
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Bus number is required")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Bus number must be 2–20 characters")]
        public string BusNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Operator is required")]
        [StringLength(80, ErrorMessage = "Operator name max 80 characters")]
        public string Operator { get; set; } = string.Empty;

        [Required(ErrorMessage = "Departure location is required")]
        [StringLength(80)]
        public string From { get; set; } = string.Empty;

        [Required(ErrorMessage = "Destination location is required")]
        [StringLength(80)]
        public string To { get; set; } = string.Empty;

        [Required(ErrorMessage = "Departure date/time is required")]
        public DateTime Departure { get; set; }

        [Required(ErrorMessage = "Fare is required")]
        [Range(1, 100000, ErrorMessage = "Fare must be positive")]
        public decimal Fare { get; set; }

        [Range(1, 200, ErrorMessage = "Seats must be between 1 and 200")]
        public int SeatsAvailable { get; set; } = 40;
    }
}
