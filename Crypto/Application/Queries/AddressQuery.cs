using Binebase.Exchange.Common.Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Application.Queries
{
    public class AddressQuery : IRequest<AddressQueryResult>
    {
        public Currency Currency { get; set; }

        public class AddressQueryHandler : IRequestHandler<AddressQuery, AddressQueryResult>
        {
            public AddressQueryHandler()
            {

            }

            public Task<AddressQueryResult> Handle(AddressQuery request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
