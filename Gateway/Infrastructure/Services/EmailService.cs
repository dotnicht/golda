using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Infrastructure.Services
{
    public class EmailService : IEmailService, IConfigurationProvider<EmailService.Configuration>, ITransient<IEmailService>
    {
        private readonly ILogger _logger;
        private readonly IOptions<Configuration> _options;

        public EmailService(ILogger<EmailService> logger, IOptions<Configuration> options) => (_logger, _options) = (logger, options);

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
                From = new EmailAddress(_options.Value.FromAddress)
            };

            msg.AddTos(emails.Select(x => new EmailAddress(x)).ToList());

            var client = new SendGridClient(_options.Value.ApiKey);
            await client.SendEmailAsync(msg);
        }

        public class Configuration
        {
            public string ApiKey { get; set; }
            public string FromAddress { get; set; }
        }
    }
}
