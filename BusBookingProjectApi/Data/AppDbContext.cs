using BusBookingProjectApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BusBookingProjectApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<EmailOtp> EmailOtps { get; set; }
        public DbSet<Payment> Payments { get; set; }

        // 🔹 New DbSet for seats
        public DbSet<BusSeat> BusSeats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Bus>()
                .HasIndex(b => b.BusNumber)
                .IsUnique();

            // 🔹 Optional: Seed seats automatically (for demo, 40 seats per bus)
            modelBuilder.Entity<BusSeat>().HasData(
            // Seed will be added dynamically in SeedData class
            );
        }
    }
}
