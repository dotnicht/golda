using AutoMapper;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class MiningInstantCommand : IRequest<MiningInstantCommandResult>
    {
        public bool Boost { get; set; }

        public class MiningInstantCommandHandler : IRequestHandler<MiningInstantCommand, MiningInstantCommandResult>
        {
            private readonly ICalculationService _calculationService;
            private readonly IAccountService _accountService;
            private readonly ICurrentUserService _currentUserService;
            private readonly IIdentityService _identityService;
            private readonly IApplicationDbContext _context;
            private readonly IDateTime _dateTime;
            private readonly IMapper _mapper;
            private readonly ILogger _logger;

            public MiningInstantCommandHandler(
                ICalculationService calculationService,
                IAccountService accountService,
                ICurrentUserService currentUserService,
                IIdentityService identityService,
                IApplicationDbContext context,
                IDateTime dateTime,
                IMapper mapper,
                ILogger<MiningInstantCommandHandler> logger)
                => (_calculationService, _accountService, _currentUserService, _identityService, _context, _dateTime, _mapper, _logger)
                    = (calculationService, accountService, currentUserService, identityService, context, dateTime, mapper, logger);

            public async Task<MiningInstantCommandResult> Handle(MiningInstantCommand request, CancellationToken cancellationToken)
            {
                var mining = _context.MiningRequests
                    .OrderByDescending(x => x.Created)
                    .FirstOrDefault(
                        x => x.Type == MiningType.Instant
                        && (x.CreatedBy == _currentUserService.UserId || x.LastModifiedBy == _currentUserService.UserId)
                        && x.Created > _dateTime.UtcNow - _calculationService.InstantTimeout);

                if (mining != null)
                {
                    throw new NotSupportedException(ErrorCode.MiningInstantTimeout);
                }

                var mapping = _calculationService.InstantBoostMapping.Select(x => new { x.Key, x.Value }).OrderBy(x => x.Key);
                var index = _context.MiningRequests.Count(x => x.CreatedBy == _currentUserService.UserId && x.Type == MiningType.Instant);

                mining = new MiningRequest
                {
                    Id = Guid.NewGuid(),
                    Type = MiningType.Instant
                };

                User currentUser = await _identityService.GetUser(_currentUserService.UserId);

                for (var i = 0; i < (request.Boost ? mapping.FirstOrDefault(x => x.Value <= index)?.Value ?? 1 : 1); i++)
                {
                    if (currentUser.ReferralId != null)
                    {
                        var ammount = _calculationService.InstantMiningFee / 100 * 5;
                        await _accountService.Debit(currentUser.ReferralId.Value, Currency.EURB, ammount, mining.Id, TransactionType.Refferal);
                    }

                    await _accountService.Credit(_currentUserService.UserId, Currency.EURB, _calculationService.InstantMiningFee, mining.Id, TransactionType.Fee);
                    mining.Amount += await _calculationService.GenerateInstantReward();
                }

                _context.MiningRequests.Add(mining);
                await _context.SaveChangesAsync();

                var result = _mapper.Map<MiningInstantCommandResult>(mining);

                if (mining.Amount > 0)
                {
                    if (currentUser.ReferralId != null)
                    {
                        var ammount = mining.Amount / 100 * 5;
                        await _accountService.Debit(currentUser.ReferralId.Value, Currency.BINE, ammount, mining.Id, TransactionType.Refferal);
                    }

                    await _accountService.Debit(_currentUserService.UserId, Currency.BINE, mining.Amount, mining.Id, TransactionType.Mining, MiningType.Instant);
                    var promotion = await _calculationService.GeneratePromotion(index);
                    if (promotion != null)
                    {
                        _context.Promotions.Add(promotion);
                        await _context.SaveChangesAsync();
                        result.Promotion = _mapper.Map<MiningInstantCommandResult.PromotionItem>(promotion);
                    }
                }

                return result;
            }
        }
    }
}
