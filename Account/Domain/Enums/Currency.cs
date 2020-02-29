using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.AccountService.Domain.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Currency
    {
        BINE = 1,
        EURB = 2,
        BTC = 3,
        ETH = 4
    }
}
