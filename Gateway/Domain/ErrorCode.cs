using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.Gateway.Domain
{
    public static class ErrorCode
    {
        /// <summary>The generic error code for different account related sign in issues. Will be spited into several in future.</summary>
        public const string GenericSignIn = "account_error";
        /// <summary>
        /// The invalid password error.
        /// </summary>
        public const string PasswordMismatch = "password_mismatch";
        public const string MultyFactor = "multy_factor";
    }
}
