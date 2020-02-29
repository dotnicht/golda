using Binebase.Exchange.Gateway.Application.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Binebase.Exchange.Gateway.Application.Common.Models
{
    public class Result
    {
        public bool Succeeded { get; }
        public string[] Errors { get; }
        private Result(bool succeeded, IEnumerable<string> errors) => (Succeeded, Errors) = (succeeded, errors.ToArray());
        public ValidationException ToValidationException(string name) => new ValidationException(new Dictionary<string, string[]> { [name] = Errors });
        public SecurityException ToSecurityException() => new SecurityException(ToString());
        public static Result Success() => new Result(true, Array.Empty<string>());
        public static Result Failure(string error) => Failure(new[] { error });
        public static Result Failure(IEnumerable<string> errors) => new Result(false, errors);
    }
}
