using AutoMapper;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.Enums;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class MiningBonusCommand : IRequest<MiningBonusCommandResult>
    {
        public class MiningBonusCommandHandler : IRequestHandler<MiningBonusCommand, MiningBonusCommandResult>
        {
            private readonly ICurrentUserService _currentUserService;
            private readonly ICalculationService _calculationService;
            private readonly IAccountService _accountService;
            private readonly IApplicationDbContext _context;
            private readonly IDateTime _dateTime;
            private readonly IMapper _mapper;

            public MiningBonusCommandHandler(
                ICurrentUserService currentUserService,
                ICalculationService calculationService,
                IAccountService accountService,
                IApplicationDbContext context,
                IDateTime dateTime,
                IMapper mapper)
                => (_currentUserService, _calculationService, _accountService, _context, _dateTime, _mapper)
                    = (currentUserService, calculationService, accountService, context, dateTime, mapper);

            public async Task<MiningBonusCommandResult> Handle(MiningBonusCommand request, CancellationToken cancellationToken)
            {
                var mining =_context.MiningRequests
                    .OrderByDescending(x => x.Created)
                    .FirstOrDefault(
                        x => x.Type == TransactionType.Weekly
                        && new[] { x.CreatedBy, x.LastModifiedBy }.Contains(_currentUserService.UserId)
                        && x.Created > _dateTime.UtcNow - _calculationService.WeeklyTimeout);

                if (mining != null)
                {
                    throw new NotSupportedException("Timeout active.");
                }

                var (amount, type) = await _calculationService.GenerateWeeklyReward();

                mining = new MiningRequest 
                { 
                    Id = Guid.NewGuid(),
                    Amount = amount,
                    Type = type
                };

                _context.MiningRequests.Add(mining);
                await _context.SaveChangesAsync();

                await _accountService.Debit(_currentUserService.UserId, Currency.BINE, mining.Amount, mining.Id, TransactionSource.Mining, mining.Type);
                (amount, type) = await _calculationService.GenerateBonusReward();

                if (type != TransactionType.Default && amount > 0)
                {
                    mining.Type = type;
                    mining.Amount += amount;
                    await _accountService.Debit(_currentUserService.UserId, Currency.BINE, amount, mining.Id, TransactionSource.Mining, type);
                }

                return _mapper.Map<MiningBonusCommandResult>(mining);
            }
        }
    }
}
