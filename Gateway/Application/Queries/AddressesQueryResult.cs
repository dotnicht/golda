using Binebase.Exchange.Gateway.Common.Domain;
using System.Collections.Generic;

namespace Binebase.Exchange.Gateway.Application.Queries
{
    public class AddressesQueryResult
    {
        public Dictionary<Currency, string> Addresses { get; set; }  
    }
}
