using Binebase.Exchange.Common.Application.Mappings;
using Binebase.Exchange.Common.Domain;

namespace Binebase.Exchange.CryptoService.Application.Queries
{
    public class AddressesQueryResult
    {
        public Address[] Addresses { get; set; }

        public class Address : IMapFrom<Domain.Entities.Address>
        {
            public Currency Currency { get; set; }
            public string Public { get; set; }
        }
    }
}
