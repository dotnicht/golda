using Binebase.Exchange.Common.Application.Interfaces;
using System;

namespace Binebase.Exchange.Common.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
