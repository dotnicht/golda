using AutoMapper;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Application.Configuration;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class MiningInstantCommand : IRequest<MiningInstantCommandResult>
    {
        public int? Boost { get; set; }

        public class MiningInstantCommandHandler : IRequestHandler<MiningInstantCommand, MiningInstantCommandResult>
        {
            private readonly ICalculationService _calculationService;
            private readonly IAccountService _accountService;
            private readonly ICurrentUserService _currentUserService;
            private readonly IIdentityService _identityService;
            private readonly IApplicationDbContext _context;
            private readonly IDateTime _dateTime;
            private readonly IMapper _mapper;
            private readonly MiningCalculation _configuration;

            public MiningInstantCommandHandler(
                ICalculationService calculationService,
                IAccountService accountService,
                ICurrentUserService currentUserService,
                IIdentityService identityService,
                IApplicationDbContext context,
                IDateTime dateTime,
                IMapper mapper,
                IOptions<MiningCalculation> options)
                => (_calculationService, _accountService, _currentUserService, _identityService, _context, _dateTime, _mapper, _configuration)
                    = (calculationService, accountService, currentUserService, identityService, context, dateTime, mapper, options.Value);

            public async Task<MiningInstantCommandResult> Handle(MiningInstantCommand request, CancellationToken cancellationToken)
            {
                var index = _context.MiningRequests.Count(x => x.CreatedBy == _currentUserService.UserId && x.Type == MiningType.Instant);

                if (request.Boost > 0 && (!_configuration.Instant.BoostMapping.Any(x => x.Value == request.Boost) || index < request.Boost))
                {
                    throw new NotSupportedException(ErrorCode.UnsupportedBoost);
                }

                if (request.Boost * _configuration.Instant.Fee > await _accountService.GetBalance(_currentUserService.UserId, Currency.EURB))
                {
                    throw new NotSupportedException(ErrorCode.InsufficientBalance);
                }

                var created = _dateTime.UtcNow - _configuration.Instant.Timeout;

                var mining = _context.MiningRequests
                    .OrderByDescending(x => x.Created)
                    .FirstOrDefault(
                        x => x.Type == MiningType.Instant
                        && (x.CreatedBy == _currentUserService.UserId || x.LastModifiedBy == _currentUserService.UserId)
                        && x.Created > created);

                if (mining != null)
                {
                    throw new NotSupportedException(ErrorCode.MiningInstantTimeout);
                }

                var mapping = _configuration.Instant.BoostMapping.Select(x => new { Key = int.Parse(x.Key), x.Value }).OrderByDescending(x => x.Key);
                var currentUser = await _identityService.GetUser(_currentUserService.UserId);
                var promotions = new List<Promotion>();
                var times = request.Boost > 0 ? mapping.FirstOrDefault(x => x.Value <= request.Boost)?.Value ?? 1 : 1;

                var amount = 0M;

                for (var i = 0; i < times; i++)
                {
                    mining = new MiningRequest
                    {
                        Id = Guid.NewGuid(),
                        Type = MiningType.Instant,
                        Amount = await _calculationService.GenerateInstantReward()
                    };

                    amount += mining.Amount;

                    if (currentUser.ReferralId != null)
                    {
                        await _accountService.Debit(currentUser.ReferralId.Value, Currency.EURB, _configuration.Instant.Fee / 100 * 5, mining.Id, TransactionType.Refferal);
                    }

                    await _accountService.Credit(_currentUserService.UserId, Currency.EURB, _configuration.Instant.Fee, mining.Id, TransactionType.Fee);

                    if (mining.Amount > 0)
                    {
                        var promotion = await _calculationService.GeneratePromotion(index, mining.Amount);
                        if (promotion != null)
                        {
                            promotion.MiningRequestId = mining.Id;
                            _context.Promotions.Add(promotion);
                            promotions.Add(promotion);
                        }
                    }

                    _context.MiningRequests.Add(mining);
                    await _context.SaveChangesAsync();

                    if (mining.Amount > 0)
                    {
                        if (currentUser.ReferralId != null)
                        {
                            await _accountService.Debit(currentUser.ReferralId.Value, Currency.BINE, mining.Amount / 100 * 5, mining.Id, TransactionType.Refferal);
                        }

                        await _accountService.Debit(_currentUserService.UserId, Currency.BINE, mining.Amount, mining.Id, TransactionType.Mining);
                    }
                }

                return new MiningInstantCommandResult 
                {
                    Id = mining.Id,
                    Index = mining.Index,
                    Amount = amount,
                    Promotions = _mapper.Map<MiningInstantCommandResult.PromotionItem[]>(promotions)
                };
            }
        }
    }
}
