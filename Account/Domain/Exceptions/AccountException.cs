using System;

namespace Binebase.Exchange.AccountService.Domain.Exceptions
{

    [Serializable]
    public class AccountException : Exception
    {
        public AccountException() { }
        public AccountException(string message) : base(message) { }
        public AccountException(string message, Exception inner) : base(message, inner) { }
        protected AccountException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
