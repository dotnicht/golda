using Binebase.Exchange.Common.Application.Mappings;
using Binebase.Exchange.Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.CryptoService.Application.Queries
{
    public class AddressesQueryResult
    {
        public Address[] Addresses { get; set; }

        public class Address : IMapFrom<Domain.Entities.Address>
        {

        }
    }
}
