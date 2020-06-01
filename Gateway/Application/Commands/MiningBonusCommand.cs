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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            private readonly MiningCalculation _configuration;

            public MiningBonusCommandHandler(
                ICurrentUserService currentUserService,
                ICalculationService calculationService,
                IAccountService accountService,
                IApplicationDbContext context,
                IDateTime dateTime,
                IMapper mapper,
                IOptions<MiningCalculation> options)
                => (_currentUserService, _calculationService, _accountService, _context, _dateTime, _mapper, _configuration)
                    = (currentUserService, calculationService, accountService, context, dateTime, mapper, options.Value);

            public async Task<MiningBonusCommandResult> Handle(MiningBonusCommand request, CancellationToken cancellationToken)
            {
                var created = _dateTime.UtcNow - _configuration.Weekly.Timeout;

                var mining =_context.MiningRequests
                    .OrderByDescending(x => x.Created)
                    .FirstOrDefault(
                        x => (x.Type == MiningType.Weekly || x.Type == MiningType.Bonus || x.Type == MiningType.Default)
                        && (x.CreatedBy == _currentUserService.UserId || x.LastModifiedBy == _currentUserService.UserId)
                        && x.Created > created);

                if (mining != null)
                {
                    throw new NotSupportedException(ErrorCode.MiningBonusTimeout);
                }

                var (amount, type) = await _calculationService.GenerateBonusReward();

                if (type == MiningType.Default && amount == 0)
                {
                    (amount, type) = await _calculationService.GenerateWeeklyReward();
                }

                mining = new MiningRequest
                {
                    Id = Guid.NewGuid(),
                    Amount = amount,
                    Type = type
                };

                await _accountService.Debit(_currentUserService.UserId, Currency.BINE, amount, mining.Id, TransactionType.Mining);

                _context.MiningRequests.Add(mining);
                await _context.SaveChangesAsync();

                return _mapper.Map<MiningBonusCommandResult>(mining);
            }
        }
    }
}
