using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Application.Commands
{
    public class TransferAssetsCommand : IRequest<TransferAssetsCommandResult>
    {
        public Currency Currency { get; set; }

        public class TransferDepositsCommandHandler : IRequestHandler<TransferAssetsCommand, TransferAssetsCommandResult>
        {
            private readonly ITransactionService _transactionService;

            public TransferDepositsCommandHandler(ITransactionService transactionService)
                => (_transactionService) = (transactionService);

            public async Task<TransferAssetsCommandResult> Handle(TransferAssetsCommand request, CancellationToken cancellationToken)
                => new TransferAssetsCommandResult { Amount = await _transactionService.TransferAssets(request.Currency) };
        }
    }
}
