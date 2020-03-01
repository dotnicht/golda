using Binebase.Exchange.Common.Application.Interfaces;
using System;

namespace Binebase.Exchange.Gateway.Infrastructure.Services
{
    public class DateTimeService : IDateTime, ITransient<IDateTime>
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
