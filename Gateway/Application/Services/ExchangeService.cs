using Binebase.Exchange.Gateway.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.Gateway.Application.Services
{
    public class ExchangeService : IExchangeService
    {
        private readonly IApplicationDbContext _context;
        private readonly IAccountService _accountService;

        public ExchangeService(IApplicationDbContext context, IAccountService accountService)
            => (_context, _accountService) = (context, accountService);

    }
}
