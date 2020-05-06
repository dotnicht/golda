using Binebase.Exchange.Common.Application.Interfaces;

namespace Binebase.Exchange.Gateway.Infrastructure.Configuration
{
    public class Phone : IConfig
    {
        public string AuthToken { get; set; }
        public string AccountSid { get; set; }
        public string VerificationServiceSID { get; set; }
        public string Channel { get; set; }

        
    }
}
