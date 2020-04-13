using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Binebase.Exchange.Common.Domain
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionType
    {
        Internal = 0,
        Mining = 1,
        Exchange = 2,
        Deposit = 3,
        Withdraw = 4,
        Refferal = 5,
        Fee = 6,
        SignUp = 7
    }
}
