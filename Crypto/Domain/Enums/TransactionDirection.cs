using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Binebase.Exchange.CryptoService.Domain.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionDirection
    {
        Internal = 0,
        Inbound = 1,
        Outbound = 2,
        Transfer = 3
    }
}
