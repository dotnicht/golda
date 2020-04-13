﻿using AutoMapper;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.Enums;
using MediatR;
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

            public MiningInstantCommandHandler(
                ICalculationService calculationService,
                IAccountService accountService,
                ICurrentUserService currentUserService,
                IIdentityService identityService,
                IApplicationDbContext context,
                IDateTime dateTime,
                IMapper mapper)
                => (_calculationService, _accountService, _currentUserService, _identityService, _context, _dateTime, _mapper)
                    = (calculationService, accountService, currentUserService, identityService, context, dateTime, mapper);

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
                var promotions = new List<Promotion>();

                for (var i = 0; i < (request.Boost != null ? mapping.FirstOrDefault(x => x.Value <= request.Boost)?.Key ?? 1 : 1); i++)
                {
                    if (currentUser.ReferralId != null)
                    {
                        var ammount = _calculationService.InstantMiningFee / 100 * 5; // TODO: move to config (referral service).
                        await _accountService.Debit(currentUser.ReferralId.Value, Currency.EURB, ammount, mining.Id, TransactionType.Refferal);
                    }

                    await _accountService.Credit(_currentUserService.UserId, Currency.EURB, _calculationService.InstantMiningFee, mining.Id, TransactionType.Fee);
                    var value = await _calculationService.GenerateInstantReward();

                    if (value > 0)
                    {
                        mining.Amount += value;
                        var promotion = await _calculationService.GeneratePromotion(index, value);
                        if (promotion != null)
                        {
                            promotion.MiningRequestId = mining.Id;
                            _context.Promotions.Add(promotion);
                            promotions.Add(promotion);
                        }
                    }
                }

                if (mining.Amount > 0)
                {
                    if (currentUser.ReferralId != null)
                    {
                        var ammount = mining.Amount / 100 * 5;
                        await _accountService.Debit(currentUser.ReferralId.Value, Currency.BINE, ammount, mining.Id, TransactionType.Refferal);
                    }

                    await _accountService.Debit(_currentUserService.UserId, Currency.BINE, mining.Amount, mining.Id, TransactionType.Mining);
                }

                _context.MiningRequests.Add(mining);
                await _context.SaveChangesAsync();

                var result = _mapper.Map<MiningInstantCommandResult>(mining);
                result.Promotions =  _mapper.Map<MiningInstantCommandResult.PromotionItem[]>(promotions);

                return result;
            }
        }
    }
}
