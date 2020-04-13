using Binebase.Exchange.Common.Application.Interfaces;

namespace Binebase.Exchange.Gateway.Infrastructure.Configuration
{
    public class Redis : IConfig
    {
        public string ConnectionString { get; set; }
    }
}
