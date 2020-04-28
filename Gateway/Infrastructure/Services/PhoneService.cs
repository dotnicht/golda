using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Verify.V2.Service;

namespace Binebase.Exchange.Gateway.Infrastructure.Services
{
    class PhoneService : IPhoneService
    {
        private readonly Phone _configuration;

        public PhoneService(IOptions<Phone> options)
        {
            _configuration = options.Value;
            TwilioClient.Init(_configuration.AccountSid, _configuration.AuthToken);
        }
        public async Task<(string Sid, bool IsValid, List<string> Errors)> CheckVerificationAsync(string phoneNumber, string code)
        {
            try
            {
                var verificationCheckResource = await VerificationCheckResource.CreateAsync(
                    to: phoneNumber,
                    code: code,
                    pathServiceSid: _configuration.VerificationServiceSID
                );

                if (verificationCheckResource.Status.Equals("approved"))
                    return (verificationCheckResource.Sid, true, null);
                else
                    return (null, false, new List<string> { "Wrong code. Try again." });
            }
            catch (TwilioException e)
            {
                return (null, false, new List<string> { e.Message });
            }
        }

        public async Task<(string Sid, bool IsValid, List<string> Errors)> StartVerificationAsync(string phoneNumber)
        {
            try
            {
                var verificationResource = await VerificationResource.CreateAsync(
                    to: phoneNumber,
                    channel: _configuration.VerificationServiceSID,
                    pathServiceSid: _configuration.VerificationServiceSID);

                return (verificationResource.Sid, true, null);
            }
            catch (TwilioException e)
            {
                return (null, false, new List<string> { e.Message });
            }
        }
    }
}
