using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Application.Queries
{
    public class AddressesQuery : IRequest<AddressesQueryResult>
    {
        public class AddressesQueryHandler : IRequestHandler<AddressesQuery, AddressesQueryResult>
        {
            public AddressesQueryHandler()
            {

            }


            public Task<AddressesQueryResult> Handle(AddressesQuery request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
