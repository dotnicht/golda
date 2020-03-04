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
    public class ExchangePromotionCommand : IRequest, IIdContainer
    {
        public Guid Id { get; set; }

        public class ExchangePromotionCommandHandler : IRequestHandler<ExchangePromotionCommand>
        {
            private readonly IApplicationDbContext _context;
            private readonly IAccountService _accountService;
            private readonly IExchangeRateService _exchangeRateService;
            private readonly ICurrentUserService _currentUserService;
            private readonly IDateTime _dateTime;

            public ExchangePromotionCommandHandler(
                IApplicationDbContext context,
                IAccountService accountService,
                IExchangeRateService exchangeRateService,
                ICurrentUserService currentUserService,
                IDateTime dateTime) 
                => (_context, _accountService, _exchangeRateService, _currentUserService, _dateTime) 
                    = (context, accountService, exchangeRateService, currentUserService, dateTime);

            public async Task<Unit> Handle(ExchangePromotionCommand request, CancellationToken cancellationToken)
            {
                var promotion = _context.Promotions.SingleOrDefault(x => x.Id == request.Id && x.Created > _dateTime.UtcNow - TimeSpan.FromDays(1) && !x.IsExchanged);

                if (promotion == null)
                {
                    throw new NotFoundException(nameof(Promotion), request.Id);
                }

                var ex = await _exchangeRateService.GetExchangeRate(new Pair(Currency.BINE, promotion.Currency));

                await _accountService.Credit(_currentUserService.UserId, Currency.BINE, promotion.TokenAmount, TransactionSource.Exchange);
                await _accountService.Debit(_currentUserService.UserId, promotion.Currency, promotion.TokenAmount * ex.Rate, TransactionSource.Exchange);

                promotion.IsExchanged = true;
                await _context.SaveChangesAsync();
                    
                return Unit.Value;
            }
        }
    }
}
