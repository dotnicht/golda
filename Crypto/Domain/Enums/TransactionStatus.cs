using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Binebase.Exchange.CryptoService.Domain.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionStatus
    {
        Published = 0,
        Confirmed = 1,
        Failed = 2
    }
}
