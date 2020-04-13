using AutoMapper;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Application.Queries
{
    public class AddressesQuery : IRequest<AddressesQueryResult>
    {
        public Guid Id { get; set; }

        public class AddressesQueryHandler : IRequestHandler<AddressesQuery, AddressesQueryResult>
        {
            private readonly IApplicationDbContext _context;
            private readonly IMapper _mapper;

            public AddressesQueryHandler(IApplicationDbContext context, IMapper mapper)
                => (_context, _mapper) = (context, mapper);

            public async Task<AddressesQueryResult> Handle(AddressesQuery request, CancellationToken cancellationToken)
            {
                var addresses = _context.Addresses.Where(x => x.AccountId == request.Id);
                return await Task.FromResult(new AddressesQueryResult { Addresses = _mapper.Map<AddressesQueryResult.Address[]>(addresses) });
            }
        }
    }
}
