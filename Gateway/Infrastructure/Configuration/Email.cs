using Binebase.Exchange.Common.Application.Interfaces;

namespace Binebase.Exchange.Gateway.Infrastructure.Configuration
{
    public class Email : IConfig
    {
        public string ApiKey { get; set; }
        public string FromAddress { get; set; }
    }
}
