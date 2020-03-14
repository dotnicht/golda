using Binebase.Exchange.Common.Domain;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Application.Commands
{
    public class PublishTransactionCommand : IRequest<PublishTransactionCommandResult>
    {
        public Currency Currency { get; set; }
        public string Address { get; set; }
        public decimal Amount { get; set; }

        public class PublishTransactionCommandHandler : IRequestHandler<PublishTransactionCommand, PublishTransactionCommandResult>
        {
            public Task<PublishTransactionCommandResult> Handle(PublishTransactionCommand request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
