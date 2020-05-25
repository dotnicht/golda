using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Application.Commands
{
    public class TransferDepositsCommand : IRequest
    {
        public class TransferDepositsCommandHandler : IRequestHandler<TransferDepositsCommand>
        {
            public TransferDepositsCommandHandler()
            {

            }

            public Task<Unit> Handle(TransferDepositsCommand request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
