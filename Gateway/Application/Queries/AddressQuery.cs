﻿using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Common.Domain;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class AddressQuery : IRequest<AddressQueryResult>
    {
        public Currency Currency { get; set; }

        public class AddressQueryHandler : IRequestHandler<AddressQuery, AddressQueryResult>
        {
            private readonly ICurrentUserService _currentUserService;
            private readonly ICryptoService _cryptoService;

            private readonly static Currency[] _currencies = new[] { Currency.BTC, Currency.ETH };

            public AddressQueryHandler(ICurrentUserService currentUserService, ICryptoService cryptoService)
                => (_currentUserService, _cryptoService) = (currentUserService, cryptoService);

            public async Task<AddressQueryResult> Handle(AddressQuery request, CancellationToken cancellationToken)
                => _currencies.Contains(request.Currency)
                    ? new AddressQueryResult { Address = await _cryptoService.GetAddress(_currentUserService.UserId, request.Currency) }
                    : throw new NotSupportedException($"Supported currencies {string.Join(", ", _currencies)}.");
        }
    }
}
