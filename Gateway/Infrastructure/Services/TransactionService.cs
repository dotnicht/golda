using Binebase.Exchange.Common.Application;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.ValueObjects;
using Binebase.Exchange.Gateway.Infrastructure.Configuration;
using Binebase.Exchange.Gateway.Infrastructure.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Infrastructure.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly Crypto _configuration;
        private readonly ILogger _logger;
        private readonly ICryptoService _cryptoService;
        private readonly IAccountService _accountService;
        private readonly IExchangeRateService _exchangeRateService;
        private readonly IServiceProvider _serviceProvider;

        public TransactionService(IOptions<Crypto> options, ILogger<TransactionService> logger, IServiceProvider serviceProvider) =>
             (_configuration, _logger, _cryptoService, _accountService, _exchangeRateService, _serviceProvider)
                = (options.Value, logger, serviceProvider.GetRequiredService<ICryptoService>(), serviceProvider.GetRequiredService<IAccountService>(), serviceProvider.GetRequiredService<IExchangeRateService>(), serviceProvider);

        public async Task SyncTransactions(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using (new ElapsedTimer(_logger, "CryptoTxProcess"))
                    {
                        using var scope = _serviceProvider.CreateScope();
                        using var users = scope.ServiceProvider.GetRequiredService<IUserContext>();
                        using var ctx = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

                        foreach (var id in users.Users.Select(x => x.Id))
                        {
                            foreach (var tx in await _cryptoService.GetTransactions(id))
                            {
                                var existing = ctx.Transactions.SingleOrDefault(x => x.Id == tx.Id);
                                if (existing == null)
                                {
                                    ctx.Transactions.Add(tx);
                                    if (tx.Type == TransactionType.Deposit)
                                    {
                                        await _accountService.Debit(id, tx.Currency, tx.Amount, tx.Id, tx.Type);
                                        var ex = await _exchangeRateService.GetExchangeRate(new Pair(Currency.EURB, tx.Currency));

                                        var op = new ExchangeOperation
                                        {
                                            CreatedBy = id,
                                            Id = tx.Id,
                                            Pair = ex.Pair,
                                            Amount = ex.Rate / tx.Amount,
                                        };

                                        await _accountService.Credit(id, tx.Currency, tx.Amount, op.Id, TransactionType.Exchange);
                                        await _accountService.Debit(id, Currency.EURB, op.Amount, op.Id, TransactionType.Exchange);

                                        ctx.ExchangeOperations.Add(op);
                                    }
                                }
                                else if (tx.Type == TransactionType.Withdraw && tx.Failed && !existing.Failed)
                                {
                                    existing.Failed = true;
                                    await _accountService.Credit(id, tx.Currency, tx.Amount, tx.Id, TransactionType.Compensating);
                                }
                            }

                            await ctx.SaveChangesAsync();
                        }
                    }

                    await Task.Delay(_configuration.TransactionsSyncTimeout);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while sync transactions.");
                }
            }
        }
    }
}
