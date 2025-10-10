
using BusBookingProjectApi.Data;
using BusBookingProjectApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusBookingProjectApi.Repositories
{
    public class BusRepository : IBusRepository
    {
        private readonly AppDbContext _context;

        public BusRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Bus?> GetByIdAsync(int id)
        {
            return await _context.Buses.FindAsync(id);
        }

        public async Task<Bus?> GetByBusNumberAsync(string busNumber)
        {
            return await _context.Buses.FirstOrDefaultAsync(b => b.BusNumber == busNumber);
        }

        public async Task<List<Bus>> GetAllBusesAsync()
        {
            return await _context.Buses.ToListAsync();
        }

        public async Task AddBusAsync(Bus bus)
        {
            _context.Buses.Add(bus);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBusAsync(Bus bus)
        {
            _context.Buses.Update(bus);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBusAsync(Bus bus)
        {
            _context.Buses.Remove(bus);
            await _context.SaveChangesAsync();
        }
    }
}
