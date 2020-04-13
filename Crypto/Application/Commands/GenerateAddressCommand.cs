using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Binebase.Exchange.CryptoService.Domain.Entities;
using Binebase.Exchange.CryptoService.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
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
            private readonly IEnumerable<IBlockchainService> _blockchainServices;
            private readonly IApplicationDbContext _context;

            public GenerateAddressCommandHandler(IEnumerable<IBlockchainService> blockchainServices, IApplicationDbContext context)
                => (_blockchainServices, _context) = (blockchainServices, context);

            public async Task<GenerateAddressCommandResult> Handle(GenerateAddressCommand request, CancellationToken cancellationToken)
            {
                var service = _blockchainServices.Single(x => x.Currency == request.Currency);
                var index = _context.Addresses.Where(x => x.Currency == request.Currency).OrderByDescending(x => x.Index).FirstOrDefault()?.Index + 1 ?? 0;
                var address = new Address
                {
                    AccountId = request.Id,
                    Currency = request.Currency,
                    Public = await service.GenerateAddress(index),
                    Type = AddressType.Deposit,
                    Index = index,
                    GeneratedBlock = await service.CurrentIndex()
                };

                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();
                return new GenerateAddressCommandResult { Address = address.Public };
            }
        }
    }
}
