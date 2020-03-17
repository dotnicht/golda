using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.Enums;
using Binebase.Exchange.Gateway.Domain.ValueObjects;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class ExchangeCommand : IRequest
    {
        public Currency Base { get; set; }
        public Currency Quote { get; set; }
        public decimal Amount { get; set; }

        public class ExchangeCommandHandler : IRequestHandler<ExchangeCommand>
        {
            private readonly IApplicationDbContext _context;
            private readonly IAccountService _accountService;
            private readonly IExchangeRateService _exchangeRateService;
            private readonly ICurrentUserService _currentUserService;
            private readonly IDateTime _dateTime;

            public ExchangeCommandHandler(
                IApplicationDbContext context,
                IAccountService accountService,
                IExchangeRateService exchangeRateService,
                ICurrentUserService currentUserService,
                IDateTime dateTime)
                => (_context, _accountService, _exchangeRateService, _currentUserService, _dateTime)
                    = (context, accountService, exchangeRateService, currentUserService, dateTime);

            public async Task<Unit> Handle(ExchangeCommand request, CancellationToken cancellationToken)
            {
                var ex = await _exchangeRateService.GetExchangeRate(new Pair(request.Base, request.Quote), false);

                if (ex == null)
                {
                    throw new NotSupportedException($"Conversions from {request.Base} to {request.Quote} not supported.");
                }

                var op = new ExchangeOperation
                {
                    Id = Guid.NewGuid(),
                    Pair = new Pair(request.Base, request.Quote),
                    Amount = request.Amount,
                };

                await _accountService.Credit(_currentUserService.UserId, request.Quote, request.Amount * ex.Rate, op.Id, TransactionSource.Exchange);
                await _accountService.Debit(_currentUserService.UserId, request.Base, request.Amount, op.Id, TransactionSource.Exchange);

                _context.ExchangeOperations.Add(op);
                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
