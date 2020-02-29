using Binebase.Exchange.AccountService.Application.Common.Interfaces;
using System;

namespace Binebase.Exchange.AccountService.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
