using Binebase.Exchange.Common.Application.Exceptions;
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
    public class PublishTransactionCommand : IRequest<PublishTransactionCommandResult>
    {
        public Guid Id { get; set; }
        public Currency Currency { get; set; }
        public string Public { get; set; }
        public decimal Amount { get; set; }
        public Guid ExternalId { get; set; } // TODO: remove external id.

        public class PublishTransactionCommandHandler : IRequestHandler<PublishTransactionCommand, PublishTransactionCommandResult>
        {
            private readonly IApplicationDbContext _context;
            private readonly IBlockchainService _blockchainService;
            private readonly IAddressService _addressService;

            public PublishTransactionCommandHandler(IApplicationDbContext context, IBlockchainService blockchainService, IAddressService addressService)
                => (_context, _blockchainService, _addressService) = (context, blockchainService, addressService);

            public async Task<PublishTransactionCommandResult> Handle(PublishTransactionCommand request, CancellationToken cancellationToken)
            {
                var address = _context.Addresses.SingleOrDefault(x => x.AccountId == request.Id && x.Currency == request.Currency && x.Type == AddressType.Deposit);
                if (address == null)
                {
                    throw new SecurityException();
                }

                if (!await _addressService.ValidateAddress(request.Currency, request.Public))
                {
                    throw new ValidationException();
                }

                address = _context.Addresses.SingleOrDefault(x => x.AccountId == request.Id && x.Currency == request.Currency && x.Type == AddressType.Withdraw && x.Public == request.Public)
                    ?? _context.Addresses.Add(new Address { AccountId = request.Id, Currency = request.Currency, Type = AddressType.Withdraw, Public = request.Public, GeneratedBlock = await _blockchainService.CurrentIndex(request.Currency) }).Entity;

                var hash = await _blockchainService.PublishTransaction(request.Currency, request.Amount, request.Public);
                return new PublishTransactionCommandResult { Hash = hash };
            }
        }
    }
}
