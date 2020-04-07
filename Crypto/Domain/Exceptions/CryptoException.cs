using System;
using System.Runtime.Serialization;

namespace Binebase.Exchange.CryptoService.Domain.Exceptions
{

    [Serializable]
    public class CryptoException : Exception
    {
        public CryptoException() { }
        public CryptoException(string message) : base(message) { }
        public CryptoException(string message, Exception inner) : base(message, inner) { }
        protected CryptoException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
