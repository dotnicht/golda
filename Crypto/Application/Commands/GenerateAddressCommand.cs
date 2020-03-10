using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Application.Commands
{
    public class GenerateAddressCommand : IRequest<GenerateAddressCommandResult>
    {
        public Guid Id { get; set; }
        public Currency Currency { get; set; }

        public class GenerateAddressCommandHandler : IRequestHandler<GenerateAddressCommand, GenerateAddressCommandResult>
        {
            private readonly IBitcoinService _bitcoinService;
            private readonly IApplicationDbContext _context;

            public GenerateAddressCommandHandler(IBitcoinService bitcoinService, IApplicationDbContext context)
                => (_bitcoinService, _context) = (bitcoinService, context);

            public async Task<GenerateAddressCommandResult> Handle(GenerateAddressCommand request, CancellationToken cancellationToken)
            {
                await _bitcoinService.GenerateKeys();

                return new GenerateAddressCommandResult { };
            }
        }
    }
}
