using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace CrediAvanzaAPI.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(string toEmail, string subject, string body)
        {
            var host = _configuration["Email:Smtp:Host"];
            var portValue = _configuration["Email:Smtp:Port"];
            var user = _configuration["Email:Smtp:Username"];
            var pass = _configuration["Email:Smtp:Password"];
            var from = _configuration["Email:Smtp:From"];
            var enableSslValue = _configuration["Email:Smtp:EnableSsl"];

            if (string.IsNullOrWhiteSpace(host) ||
                string.IsNullOrWhiteSpace(portValue) ||
                string.IsNullOrWhiteSpace(user) ||
                string.IsNullOrWhiteSpace(pass) ||
                string.IsNullOrWhiteSpace(from))
            {
                throw new InvalidOperationException("Configuración de correo incompleta. Verifica Email:Smtp en appsettings.");
            }

            if (!int.TryParse(portValue, out var port))
            {
                throw new InvalidOperationException("Email:Smtp:Port inválido.");
            }

            var enableSsl = true;
            if (!string.IsNullOrWhiteSpace(enableSslValue) && bool.TryParse(enableSslValue, out var parsedSsl))
            {
                enableSsl = parsedSsl;
            }

            using var message = new MailMessage(from, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(user, pass)
            };

            await client.SendMailAsync(message);
        }
    }
}
