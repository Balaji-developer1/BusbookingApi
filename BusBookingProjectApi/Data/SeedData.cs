using BusBookingProjectApi.Data;
using BusBookingProjectApi.Models;
using System.Linq;

namespace BusBookingProjectApi
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            // Check if seats already exist
            if (!context.BusSeats.Any())
            {
                foreach (var bus in context.Buses)
                {
                    for (int i = 1; i <= bus.SeatsAvailable; i++)
                    {
                        context.BusSeats.Add(new BusSeat
                        {
                            BusId = bus.Id,
                            SeatNumber = i,
                            IsBooked = false
                        });
                    }
                }
                context.SaveChanges();
            }
        }
    }
}
