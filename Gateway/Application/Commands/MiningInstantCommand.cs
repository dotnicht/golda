﻿using AutoMapper;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

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
            private readonly IApplicationDbContext _context;
            private readonly IMapper _mapper;
            private readonly ILogger _logger;

            public MiningInstantCommandHandler(
                ICalculationService calculationService,
                IAccountService accountService,
                ICurrentUserService currentUserService,
                IApplicationDbContext context,
                IMapper mapper,
                ILogger<MiningInstantCommandHandler> logger)
                => (_calculationService, _accountService, _currentUserService, _context, _mapper, _logger)
                    = (calculationService, accountService, currentUserService, context, mapper, logger);

            public async Task<MiningInstantCommandResult> Handle(MiningInstantCommand request, CancellationToken cancellationToken)
            {
                var timeout = await _calculationService.GetInstantTimeout();
                if (timeout > default(TimeSpan))
                {
                    throw new NotSupportedException("Instant mining timeout is active.");
                }

                var result = new MiningInstantCommandResult();
                var mapping = (await _calculationService.GetInstantBoostMapping()).Select(x => new { x.Key, x.Value }).OrderBy(x => x.Key);
                var index = await _calculationService.GetCurrentMiningCount();

                for (var i = 0; i < (request.Boost ? mapping.FirstOrDefault(x => x.Value <= index)?.Value ?? 1 : 1); i++)
                {
                    var fee = await _calculationService.GetInstantMiningFee();
                    await _accountService.Credit(_currentUserService.UserId, Currency.EURB, fee, TransactionSource.Fee, TransactionType.Instant);
                    result.Amount += await _calculationService.GenerateInstantMiningReward();
                }

                if (result.Amount > 0)
                {
                    result.Id = await _accountService.Debit(_currentUserService.UserId, Currency.BINE, result.Amount, TransactionSource.Mining, TransactionType.Instant);
                    var promotion = await _calculationService.GeneratePromotion();
                    if (promotion != null)
                    {
                        _context.Promotions.Add(promotion);
                        await _context.SaveChangesAsync();
                        result.Promotion = _mapper.Map<MiningInstantCommandResult.PromotionItem>(promotion);
                        _logger.LogInformation($"Promotion item {promotion.Id} created.");
                    }
                }

                return result;
            }
        }
    }
}
