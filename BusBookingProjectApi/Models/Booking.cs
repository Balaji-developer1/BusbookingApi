using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BusBookingProjectApi.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int BusId { get; set; }

        [Required]
        [Range(1, 20)]
        public int Seats { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        public string? PaymentTransactionId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [ForeignKey(nameof(BusId))]
        public Bus? Bus { get; set; }

        [Column("SeatNumbers")]
        public string? SeatNumbersString { get; set; }

        [NotMapped]
        public List<int> SeatNumbers
        {
            get
            {
                if (string.IsNullOrWhiteSpace(SeatNumbersString))
                    return new List<int>();
                var list = new List<int>();
                foreach (var token in SeatNumbersString.Split(','))
                {
                    if (int.TryParse(token.Trim(), out var n))
                    {
                        if (!list.Contains(n))
                            list.Add(n);
                    }
                }
                return list;
            }
            set
            {
                if (value == null || !value.Any())
                {
                    SeatNumbersString = null;
                }
                else
                {
                    SeatNumbersString = string.Join(",", value.Distinct().ToList());
                }
            }
        }
    }
}
