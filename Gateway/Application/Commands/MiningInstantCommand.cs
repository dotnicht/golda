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
                var mining = _context.MiningRequests
                    .OrderByDescending(x => x.Created)
                    .FirstOrDefault(
                        x => x.Type == MiningType.Instant
                        && (x.CreatedBy == _currentUserService.UserId || x.LastModifiedBy == _currentUserService.UserId)
                        && x.Created > _dateTime.UtcNow - _configuration.Instant.Timeout);

                if (mining != null)
                {
                    throw new NotSupportedException(ErrorCode.MiningInstantTimeout);
                }

                var mapping = _configuration.Instant.BoostMapping.Select(x => new { Key = int.Parse(x.Key), x.Value }).OrderBy(x => x.Key);
                var index = _context.MiningRequests.Count(x => x.CreatedBy == _currentUserService.UserId && x.Type == MiningType.Instant);

                if (index < mapping.SingleOrDefault(x => x.Value == request.Boost)?.Key)
                {
                    throw new NotSupportedException(ErrorCode.InsufficientMinings);
                }

                mining = new MiningRequest
                {
                    Id = Guid.NewGuid(),
                    Type = MiningType.Instant
                };

                var currentUser = await _identityService.GetUser(_currentUserService.UserId);

                for (var i = 0; i < (request.Boost != null ? mapping.FirstOrDefault(x => x.Value <= request.Boost)?.Key ?? 1 : 1); i++)
                {
                    if (currentUser.ReferralId != null)
                    {
                        var ammount = _configuration.Instant.Fee / 100 * 5; // TODO: move to config (referral service).
                        await _accountService.Debit(currentUser.ReferralId.Value, Currency.EURB, ammount, mining.Id, TransactionType.Refferal);
                    }

                    await _accountService.Credit(_currentUserService.UserId, Currency.EURB, _configuration.Instant.Fee, mining.Id, TransactionType.Fee);
                    mining.Amount += await _calculationService.GenerateInstantReward();
                }

                _context.MiningRequests.Add(mining);
                await _context.SaveChangesAsync();

                var result = _mapper.Map<MiningInstantCommandResult>(mining);
                var promotions = new List<Promotion>();

                if (mining.Amount > 0)
                {
                    mining.Balance = mining.Amount + _context.MiningRequests.Where(x => x.CreatedBy == _currentUserService.UserId).Sum(x => x.Amount);

                    if (currentUser.ReferralId != null)
                    {
                        var ammount = mining.Amount / 100 * 5;
                        await _accountService.Debit(currentUser.ReferralId.Value, Currency.BINE, ammount, mining.Id, TransactionType.Refferal);
                    }

                    await _accountService.Debit(_currentUserService.UserId, Currency.BINE, mining.Amount, mining.Id, TransactionType.Mining);
                    var promotion = await _calculationService.GeneratePromotion(index);
                    if (promotion != null)
                    {
                        promotion.MiningRequest = mining;
                        _context.Promotions.Add(promotion);
                        await _context.SaveChangesAsync();
                        promotions.Add(promotion);
                    }
                }

                result.Promotions =  _mapper.Map<MiningInstantCommandResult.PromotionItem[]>(promotions);

                return result;
            }
        }
    }
}
