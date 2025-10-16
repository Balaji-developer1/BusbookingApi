using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BusBookingProjectApi.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _config;//acces the app.setting json file like a database
        public SmtpEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var host = _config["Email:SmtpHost"];
            var port = int.Parse(_config["Email:SmtpPort"]);
            var user = _config["Email:Username"];
            var pass = _config["Email:Password"];

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(user, pass),
                EnableSsl = true
            };

            var mail = new MailMessage(user, to, subject, body);
            await client.SendMailAsync(mail);
        }
    }
}
