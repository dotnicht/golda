﻿using Binebase.Exchange.Gateway.Application.Interfaces;
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

        public Task SendDepositNotificationEmail(string[] emails, string subject, string amount1, string amount2)
        {
            return SendEmail(emails, subject, string.Empty, _configuration.DepositConfirmTemplateKey, new TemplateData { Amount1 = amount1, Amount2 = amount2 });
        }

        public Task SendErrorNotificationEmail(string[] emails, string subject, string message)
        {
            return SendEmail(emails, subject, message, _configuration.ErrorNotificationTemplateKey, new TemplateData());
        }

        public Task SendResetPasswordEmail(string[] emails, string subject, string message)
        {
            return SendEmail(emails, subject, message, _configuration.ResetPasswordTemplateKey, new TemplateData());
        }

        public Task SendWithdrawNotificationEmail(string[] emails, string subject, string amount)
        {
            return SendEmail(emails, subject, string.Empty, _configuration.WithdrawRequestTemplateKey, new TemplateData { Amount1 = amount });
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

            var msg = new SendGridMessage
            {
                From = new EmailAddress(_configuration.FromAddress),
                TemplateId = templateId
            };

            msg.SetTemplateData(templateData);
            msg.AddTos(emails.Select(x => new EmailAddress(x)).ToList());

            var client = new SendGridClient(_configuration.ApiKey);
            var result = await client.SendEmailAsync(msg);

            _logger.LogDebug("SendGrid response {status}.", result.StatusCode);
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
