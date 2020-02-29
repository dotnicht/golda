using Binebase.Exchange.CryptoService.Application.Common.Interfaces;
using System;

namespace Binebase.Exchange.CryptoService.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
