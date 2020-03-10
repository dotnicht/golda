using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Binebase.Exchange.Gateway.Domain.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionSource
    {
        Internal = 0,
        Mining = 1,
        Exchange = 2,
        Deposit = 3,
        Widthraw = 4,
        Refferal = 5,
        Fee = 6
    }
}
