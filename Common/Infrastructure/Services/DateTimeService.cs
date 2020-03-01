using Binebase.Exchange.Common.Application.Common.Interfaces;
using System;

namespace Binebase.Exchange.Common.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
