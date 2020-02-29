using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Binebase.Exchange.Common.Domain
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Currency : int
    {
        BINE = 0,
        EURB = 1,
        BTC = 2,
        ETH = 3,
        EUR = int.MaxValue
    }
}
