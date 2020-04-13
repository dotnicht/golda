using Binebase.Exchange.Common.Application.Interfaces;

namespace Binebase.Exchange.Gateway.Infrastructure.Configuration
{
    public class Identity : IConfig
    {
        public string AuthSecret { get; set; }
        public string ConfirmationUrlFormat { get; set; }
        public string ResetPasswordUrlFormat { get; set; }
        public string AuthenticatorUrlFormat { get; set; }
    }
}
