﻿using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Application.Configuration;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.Enums;
using Binebase.Exchange.Gateway.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class ExchangeCommand : IRequest
    {
        public Currency Base { get; set; }
        public Currency Quote { get; set; }
        public decimal? BaseAmount { get; set; }
        public decimal? QuoteAmount { get; set; }

        public class ExchangeCommandHandler : IRequestHandler<ExchangeCommand>
        {
            private readonly IApplicationDbContext _context;
            private readonly IAccountService _accountService;
            private readonly IExchangeRateService _exchangeRateService;
            private readonly ICurrentUserService _currentUserService;
            private readonly CryptoOperations _configuration;

            public ExchangeCommandHandler(
                IApplicationDbContext context,
                IAccountService accountService,
                IExchangeRateService exchangeRateService,
                ICurrentUserService currentUserService,
                IOptions<CryptoOperations> options)
                => (_context, _accountService, _exchangeRateService, _currentUserService, _configuration)
                    = (context, accountService, exchangeRateService, currentUserService, options.Value);

            public async Task<Unit> Handle(ExchangeCommand request, CancellationToken cancellationToken)
            {
                if (request.BaseAmount > 0 || request.QuoteAmount > 0)
                {
                    var ex = await _exchangeRateService.GetExchangeRate(new Pair(request.Base, request.Quote), false, true);

                    if (_configuration.ExchangeMiningRequirement > 0
                        && _context.MiningRequests.Count(x => x.CreatedBy == _currentUserService.UserId && x.Type == MiningType.Instant) < _configuration.ExchangeMiningRequirement)
                    {
                        throw new NotSupportedException(ErrorCode.InsufficientMinings);
                    }

                    var op = new ExchangeOperation
                    {
                        Id = Guid.NewGuid(),
                        Pair = new Pair(request.Base, request.Quote),
                        Amount = request.Amount,
                    };

                    await _accountService.Credit(_currentUserService.UserId, request.Quote, request.Amount * ex.Rate, op.Id, TransactionType.Exchange);
                    await _accountService.Debit(_currentUserService.UserId, request.Base, request.Amount, op.Id, TransactionType.Exchange);

                    _context.ExchangeOperations.Add(op);
                    await _context.SaveChangesAsync();

                    return Unit.Value;
                }

                throw new NotSupportedException(ErrorCode.ExchangeOperationInvalid);
            }
        }
    }
}
