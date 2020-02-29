using Binebase.Exchange.Gateway.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class AddressesQuery : IRequest<AddressesQueryResult>
    {
        public class AddressesQueryHandler : IRequestHandler<AddressesQuery, AddressesQueryResult>
        {
            private readonly ICryptoService _cryptoService;
            private readonly ICurrentUserService _currentUserService;

            public AddressesQueryHandler(ICurrentUserService currentUserService, ICryptoService cryptoService)
                => (_currentUserService, _cryptoService) = (currentUserService, cryptoService);

            public async Task<AddressesQueryResult> Handle(AddressesQuery request, CancellationToken cancellationToken)
                => new AddressesQueryResult { Addresses = await _cryptoService.GetAddresses(_currentUserService.UserId) };
        }
    }
}
