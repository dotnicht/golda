using Binebase.Exchange.Gateway.Common.Domain;
using System;
using System.Collections.Generic;

namespace Binebase.Exchange.Gateway.Domain.ValueObjects
{
    public sealed class Pair : ValueObject
    {
        public Currency Base { get; private set; }
        public Currency Quote { get; private set; }

        public Pair(Currency @base, Currency quote) => (Base, Quote) = (@base, quote);

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Base;
            yield return Quote;
        }

        public override string ToString()
        {
            return $"{Base}/{Quote}";
        }

        public static Pair Parse(string symbol)
        {
            if (symbol is null)
            {
                throw new ArgumentNullException(nameof(symbol));
            }

            var currencies = symbol.Split("/");

            if (currencies.Length == 2)
            {
                return new Pair(Enum.Parse<Currency>(currencies[0]), Enum.Parse<Currency>(currencies[1]));
            }

            throw new ArgumentException(nameof(symbol), "Invalid symbol format");
        }
    }
}
