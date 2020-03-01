using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace Binebase.Exchange.Common.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public IDictionary<string, string[]> Failures { get; } = new Dictionary<string, string[]>();
        public ValidationException() : base("One or more validation failures have occurred.") { }
        public ValidationException(IDictionary<string, string[]> failures) : this() => Failures = failures ?? throw new ArgumentNullException(nameof(failures));
        public ValidationException(List<ValidationFailure> failures) : this()
        {
            if (failures is null)
            {
                throw new ArgumentNullException(nameof(failures));
            }

            foreach (var failureGroup in failures.GroupBy(e => e.PropertyName, e => e.ErrorMessage))
            {
                var propertyName = failureGroup.Key;
                var propertyFailures = failureGroup.ToArray();
                Failures.Add(propertyName, propertyFailures);
            }
        }
    }
}
