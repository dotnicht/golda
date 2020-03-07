using NEventStore.Domain;
using NEventStore.Domain.Persistence;
using System;
using System.Reflection;

namespace Binebase.Exchange.AccountService.Domain.Aggregates
{
    public class AggregateFactory : IConstructAggregates
    {
        public IAggregate Build(Type type, Guid id, IMemento snapshot)
        {
            var constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Array.Empty<Type>(), null);
            var aggregate = constructor.Invoke(Array.Empty<object>()) as IAggregate;
            type.GetProperty(nameof(IAggregate.Id)).SetValue(aggregate, id);
            return aggregate;
        }
    }
}
