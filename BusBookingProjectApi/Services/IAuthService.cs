using BusBookingProjectApi.Models;
using System.Threading.Tasks;

namespace BusBookingProjectApi.Services
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(string username, string email, string password, string role);
        Task<bool> VerifyOtpAsync(string email, string otp);
        Task<string?> LoginAsync(string email, string password);
        Task<string> GenerateOtpForUserAsync(User user);
    }
}
