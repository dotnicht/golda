using Binebase.Exchange.AccountService.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Binebase.Exchange.AccountService.Domain.ValueObjects
{
    public sealed class Currency : ValueObject
    {
        private static Lazy<Dictionary<string, Currency>> Supported => new Lazy<Dictionary<string, Currency>>(Factory, LazyThreadSafetyMode.ExecutionAndPublication);

        private static Dictionary<string, Currency> Factory() => typeof(Currency)
            .GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(x => x.PropertyType == typeof(Currency))
            .Select(x => x.GetValue(null))
            .Cast<Currency>()
            .ToDictionary(x => x.Code, x => x);

        public static IEnumerable<Currency> All => Supported.Value.Values;
        public static Currency EURB { get; } = new Currency("EURB");
        public static Currency BINE { get; } = new Currency("BINE");
        public static Currency BTC { get; } = new Currency("BTC");
        public static Currency ETH { get; } = new Currency("ETH");

        public string Code { get; }

        private Currency(string code) => Code = code;

        public static bool TryParse(string code, out Currency currency)
        {
            if (Supported.Value.ContainsKey(code))
            {
                currency = Supported.Value[code];
                return true;
            }

            currency = null;
            return false;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Code;
        }
    }
}
