﻿using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class SignUpCommand : IRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Referral { get; set; }
        public Guid? MiningRequestId { get; set; }

        public class SignUpCommandHandler : IRequestHandler<SignUpCommand>
        {
            private readonly IIdentityService _identityService;
            private readonly IEmailService _emailService;
            private readonly ICalculationService _calculationService;
            private readonly IAccountService _accountService;
            private readonly ICryptoService _cryptoService;
            private readonly IDateTime _dateTime;
            private readonly IApplicationDbContext _context;
            private readonly ILogger _logger;

            public SignUpCommandHandler(
                IIdentityService identityService,
                IEmailService emailService,
                ICalculationService calculationService,
                IAccountService accountService,
                ICryptoService cryptoService,
                IDateTime dateTime,
                IApplicationDbContext context,
                ILogger<SignUpCommandHandler> logger)
                => (_identityService, _emailService, _calculationService, _accountService, _cryptoService, _dateTime, _context, _logger)
                    = (identityService, emailService, calculationService, accountService, cryptoService, dateTime, context, logger);

            public async Task<Unit> Handle(SignUpCommand request, CancellationToken cancellationToken)
            {
                var (result, userId) = await _identityService.CreateUser(request.Email, request.Password);
                if (!result.Succeeded)
                {
                    throw result.ToValidationException(nameof(SignUpCommandHandler));
                }

                var ticket = await _identityService.GenerateConfirmationToken(userId);
                await _emailService.SendEmail(new[] { request.Email }, "Email Confirmation", await _identityService.GenerateConfirmationUrl(userId));

                await _accountService.Create(userId);
                await _accountService.AddCurrency(userId, Currency.BINE);
                await _accountService.AddCurrency(userId, Currency.EURB);
                await _accountService.AddCurrency(userId, Currency.BTC);
                await _accountService.AddCurrency(userId, Currency.ETH);

                await _cryptoService.GenerateAddress(userId, Currency.BTC);
                await _cryptoService.GenerateAddress(userId, Currency.ETH);

                var mining = _context.MiningRequests.SingleOrDefault(x => x.Id == request.MiningRequestId);
                if (mining != null && mining.Created + _calculationService.MiningRequestWindow <= _dateTime.UtcNow && mining.IsAnomymous)
                {
                    await _accountService.Debit(userId, Currency.BINE, mining.Amount, mining.Id, TransactionSource.Mining, TransactionType.Default);
                    mining.LastModifiedBy = userId;
                    mining.IsAnomymous = false;
                    await _context.SaveChangesAsync();
                }

                return Unit.Value;
            }
        }
    }
}
