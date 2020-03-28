using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using Binebase.Exchange.CryptoService.Domain.Entities;
using Binebase.Exchange.CryptoService.Domain.Enums;
using MediatR;
using System;
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
            private readonly IAddressService _addressService;
            private readonly IApplicationDbContext _context;

            public GenerateAddressCommandHandler(IAddressService addressService, IApplicationDbContext context)
                => (_addressService, _context) = (addressService, context);

            public async Task<GenerateAddressCommandResult> Handle(GenerateAddressCommand request, CancellationToken cancellationToken)
            {
                var index = _context.Addresses.SingleOrDefault(x => x.AccountId == request.Id && x.Currency == request.Currency)?.Index + 1 ?? 0;
                var address = new Address
                {
                    AccountId = request.Id,
                    Currency = request.Currency,
                    Public = await _addressService.GenerateAddress(request.Currency, index),
                    Type = AddressType.Deposit,
                    Index = index
                };

                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();
                return new GenerateAddressCommandResult { Address = address.Public };
            }
        }
    }
}
