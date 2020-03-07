using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Binebase.Exchange.Gateway.Domain.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionType
    {
        Default = 0,
        Weekly = 1,
        Bonus = 2,
        Instant = 3
    }
}
