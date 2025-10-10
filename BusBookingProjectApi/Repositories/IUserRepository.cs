using BusBookingProjectApi.Models;
using System.Threading.Tasks;

namespace BusBookingProjectApi.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task AddUserAsync(User user);
        Task UpdateUserAsync(User user);
    }
}
