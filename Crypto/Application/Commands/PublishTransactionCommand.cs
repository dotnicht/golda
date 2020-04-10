using Binebase.Exchange.Common.Application.Exceptions;
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
    public class PublishTransactionCommand : IRequest<PublishTransactionCommandResult>
    {
        public Guid Id { get; set; }
        public Currency Currency { get; set; }
        public string Public { get; set; }
        public decimal Amount { get; set; }
        public Guid ExternalId { get; set; }

        public class PublishTransactionCommandHandler : IRequestHandler<PublishTransactionCommand, PublishTransactionCommandResult>
        {
            private readonly IApplicationDbContext _context;
            private readonly IEnumerable<IBlockchainService> _blockchainServices;

            public PublishTransactionCommandHandler(IApplicationDbContext context, IEnumerable<IBlockchainService> blockchainServices)
                => (_context, _blockchainServices) = (context, blockchainServices);

            public async Task<PublishTransactionCommandResult> Handle(PublishTransactionCommand request, CancellationToken cancellationToken)
            {
                var address = _context.Addresses.SingleOrDefault(x => x.AccountId == request.Id && x.Currency == request.Currency && x.Type == AddressType.Deposit);
                if (address == null)
                {
                    throw new SecurityException();
                }

                var service = _blockchainServices.Single(x => x.Currency == request.Currency);

                if (!await service.ValidateAddress(request.Public))
                {
                    throw new ValidationException();
                }

                var index = await service.CurrentIndex();

                address = _context.Addresses.SingleOrDefault(x => x.AccountId == request.Id && x.Currency == request.Currency && x.Type == AddressType.Withdraw && x.Public == request.Public)
                    ?? _context.Addresses.Add(new Address { AccountId = request.Id, Currency = request.Currency, Type = AddressType.Withdraw, Public = request.Public, GeneratedBlock = index }).Entity;

                await _context.SaveChangesAsync();

                var result = await service.PublishTransaction(request.Amount, request.Public);

                var tx = new Transaction
                { 
                    Id = request.ExternalId,
                    AddressId = address.Id,
                    Amount = request.Amount,
                    RawAmount = result.Amount,
                    Hash = result.Hash,
                    Direction = TransactionDirection.Outbound
                };

                _context.Transactions.Add(tx);
                await _context.SaveChangesAsync();

                return new PublishTransactionCommandResult { Hash = result.Hash };
            }
        }
    }
}
