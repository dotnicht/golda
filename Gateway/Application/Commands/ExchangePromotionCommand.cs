using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.ValueObjects;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class ExchangePromotionCommand : IRequest
    {
        public Guid[] Promotions { get; set; }

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
                // TODO: timeout value to config. 
                foreach (var promotion in _context.Promotions.Where(x => request.Promotions.Contains(x.Id) && x.Created > _dateTime.UtcNow - TimeSpan.FromDays(1) && !x.IsExchanged))
                {
                    var ex = await _exchangeRateService.GetExchangeRate(new Pair(Currency.BINE, promotion.Currency), false);
                    await _accountService.Credit(_currentUserService.UserId, Currency.BINE, promotion.TokenAmount, promotion.Id, TransactionType.Exchange);
                    await _accountService.Debit(_currentUserService.UserId, promotion.Currency, promotion.TokenAmount * ex.Rate, promotion.Id, TransactionType.Exchange);

                    promotion.IsExchanged = true;
                    await _context.SaveChangesAsync();
                }

                return Unit.Value;
            }
        }
    }
}
