using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusBookingProjectApi.Models
{
    public class BusSeat
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BusId { get; set; }

        [Required]
        public int SeatNumber { get; set; }

        public bool IsBooked { get; set; } = false;

        public int? BookedByUserId { get; set; }

        [ForeignKey("BusId")]
        public Bus? Bus { get; set; }
    }
}
