using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Binebase.Exchange.CryptoService.Domain.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AddressType
    {
        Internal = 0,
        Deposit = 1,
        Withdraw = 2
    }
}
