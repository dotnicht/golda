using Binebase.Exchange.Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.CryptoService.Application.Queries
{
    public class AddressesQueryResult
    {
        public Dictionary<Currency, string> Addresses { get; set; }
    }
}
