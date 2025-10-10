using BusBookingProjectApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusBookingProjectApi.Repositories
{
    public interface IBusRepository
    {
        Task<Bus?> GetByIdAsync(int id);
        Task<Bus?> GetByBusNumberAsync(string busNumber);
        Task<List<Bus>> GetAllBusesAsync();
        Task AddBusAsync(Bus bus);
        Task UpdateBusAsync(Bus bus);
        Task DeleteBusAsync(Bus bus);
    }
}
