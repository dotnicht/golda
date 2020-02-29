using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Binebase.Exchange.Gateway.Domain.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionSource
    {
        Mining = 0,
        Exchange = 1,
        Deposit = 2,
        Widthraw = 3,
        Refferal = 4,
        Fee = 5
    }
}
