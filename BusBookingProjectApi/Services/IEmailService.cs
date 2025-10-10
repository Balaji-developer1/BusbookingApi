using System.Threading.Tasks;

namespace BusBookingProjectApi.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
