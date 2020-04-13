using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger _logger;
        private readonly Email _configuration;

        public EmailService(ILogger<EmailService> logger, IOptions<Email> options) => (_logger, _configuration) = (logger, options.Value);

        public async Task SendEmail(string[] emails, string subject, string message)
        {
            if (emails is null)
            {
                throw new ArgumentNullException(nameof(emails));
            }

            if (subject is null)
            {
                throw new ArgumentNullException(nameof(subject));
            }

            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var msg = new SendGridMessage
            {
                Subject = subject,
                HtmlContent = message,
                From = new EmailAddress(_configuration.FromAddress)
            };

            msg.AddTos(emails.Select(x => new EmailAddress(x)).ToList());

            var client = new SendGridClient(_configuration.ApiKey);
            var result = await client.SendEmailAsync(msg);
            _logger.LogDebug($"SendGrid response {result.StatusCode}.");
        }
    }
}
