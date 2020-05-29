using Binebase.Exchange.Common.Domain;
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

        public Task SendConfirmRegistrationEmail(string[] emails, string subject, string message)
        {
            return SendEmail(emails, subject, message, _configuration.ConfirmRegistrationTemplateKey, new TemplateData());
        }

        public Task SendDepositNotificationEmail(string[] emails, string subject, decimal amount1, Currency currency1, decimal amount2, Currency currency2)
        {
            return SendEmail(emails, subject, string.Empty, _configuration.DepositConfirmTemplateKey,
                                new TemplateData
                                {
                                    Amount1 = ToStringWithCurrencyPrecision(amount1, currency1),
                                    Amount2 = ToStringWithCurrencyPrecision(amount2, currency2)
                                });
        }

        public Task SendWithdrawNotificationEmail(string[] emails, string subject, decimal amount1, Currency currency1)
        {
            return SendEmail(emails, subject, string.Empty, _configuration.WithdrawRequestTemplateKey,
                                new TemplateData { Amount1 = ToStringWithCurrencyPrecision(amount1, currency1) });
        }

        public Task SendErrorNotificationEmail(string[] emails, string subject, string message)
        {
            return SendEmail(emails, subject, message, _configuration.ErrorNotificationTemplateKey, new TemplateData());
        }

        public Task SendResetPasswordEmail(string[] emails, string subject, string message)
        {
            return SendEmail(emails, subject, message, _configuration.ResetPasswordTemplateKey, new TemplateData());
        }

        private async Task SendEmail(string[] emails, string subject, string message, string templateId, TemplateData templateData)
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

            templateData.Subject = subject;
            templateData.Name = emails[0];
            templateData.Message = message;

            var msg = MailHelper.CreateSingleTemplateEmail(new EmailAddress(_configuration.FromAddress, "Binebase Support"), new EmailAddress(emails[0]), templateId, templateData);

            var client = new SendGridClient(_configuration.ApiKey);
            var result = await client.SendEmailAsync(msg);

            _logger.LogDebug("SendGrid response {status}.", result.StatusCode);
        }

        private string ToStringWithCurrencyPrecision(decimal amount, Currency currency)
        {
            string strAmount = amount.ToString();
            int dotIndex = strAmount.IndexOf('.');

            string result = currency switch
            {
                Currency.BINE => $"{strAmount.Substring(0, dotIndex + 2)} {currency}",
                Currency.EURB => $"{strAmount.Substring(0, dotIndex + 3)} {currency}",
                Currency.BTC => $"{strAmount.Substring(0, dotIndex + 7)} {currency}",
                Currency.ETH => $"{strAmount.Substring(0, dotIndex + 5)} {currency}",
                _ => $"{amount} {currency}",
            };
            return result;
        }

        private class TemplateData
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
