using Binebase.Exchange.Gateway.Application.Enums;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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

        public EmailService(ILogger<EmailService> logger, IOptions<Email> options) 
            => (_logger, _configuration) = (logger, options.Value);

        public async Task SendEmail(string[] emails, string subject, string message, EmailType emailType)
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

            var templateId = emailType switch
            {
                EmailType.ConfirmRegistration => _configuration.ConfirmRegistrationTemplateKey,
                EmailType.DepositNotification => _configuration.DepositConfirmTemplateKey,
                EmailType.ResetPassword => _configuration.ResetPasswordTemplateKey,
                EmailType.WithdrawNotification => _configuration.WithdrawRequestTemplateKey,
                EmailType.ErrorNotification => _configuration.ErrorNotificationTemplateKey,
                _ => string.Empty,
            };

            var dynamicTemplateData = new ExampleTemplateData
            {
                Subject = subject,
                Name = emails[0],
                Message = message
            };

            if (emailType == EmailType.DepositNotification)
            {
                var sp = message.Split(";");
                dynamicTemplateData.Amount1 = sp.First();
                dynamicTemplateData.Amount2 = sp.Last();
            }

            if (emailType == EmailType.WithdrawNotification)
            {
                dynamicTemplateData.Amount1 = message;
            }

            var msg = new SendGridMessage
            {
                From = new EmailAddress(_configuration.FromAddress),
                TemplateId = templateId
            };

            msg.SetTemplateData(dynamicTemplateData);

            msg.AddTos(emails.Select(x => new EmailAddress(x)).ToList());

            var client = new SendGridClient(_configuration.ApiKey);
            var result = await client.SendEmailAsync(msg);

            _logger.LogDebug("SendGrid response {status}.", result.StatusCode);
        }

        private class ExampleTemplateData
        {
            [JsonProperty("subject")]
            public string Subject { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("amount1")]
            public string Amount1 { get; set; }

            [JsonProperty("amount2")]
            public string Amount2 { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }
        }
    }
}
