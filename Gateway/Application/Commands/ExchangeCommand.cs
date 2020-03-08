using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.Enums;
using Binebase.Exchange.Gateway.Domain.ValueObjects;
using MediatR;
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
        public decimal Amount { get; set; }
        public Guid? ReferenceId { get; set; }

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
                if (request.ReferenceId == null)
                {
                    throw new NotSupportedException();
                }

                var promotion = _context.Promotions.SingleOrDefault(
                    x => x.Id == request.ReferenceId.Value 
                    && x.Created > _dateTime.UtcNow - TimeSpan.FromDays(1) 
                    && !x.IsExchanged
                    && x.CreatedBy == _currentUserService.UserId);

                if (promotion == null)
                {
                    throw new NotFoundException(nameof(Promotion), request.ReferenceId);
                }

                var ex = await _exchangeRateService.GetExchangeRate(new Pair(Currency.BINE, promotion.Currency));

                await _accountService.Credit(_currentUserService.UserId, Currency.BINE, promotion.TokenAmount, promotion.Id, TransactionSource.Exchange);
                await _accountService.Debit(_currentUserService.UserId, promotion.Currency, promotion.TokenAmount * ex.Rate, promotion.Id, TransactionSource.Exchange);

                promotion.IsExchanged = true;
                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
