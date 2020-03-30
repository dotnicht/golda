using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.Gateway.Application
{
    public static class ErrorCode
    {
        /// <summary>
        /// The generic error code for different account related sign in issues. Will be spited into several in future.
        /// </summary>
        public const string Account = "account_error";
        /// <summary>
        /// The invalid password error.
        /// </summary>
        public const string PasswordMismatch = "password_mismatch";
        /// <summary>
        /// The generic error related to multi factor code or password validation.
        /// </summary>
        public const string MultiFactor = "multi_factor";
        public const string MiningInstantTimeout = "mining_instant_timeout";
        public const string MiningBonusTimeout = "mining_bonus_timeout";
        public const string MiningRequestNotSupported = "mining_request_not_supported";
        public const string ExchangeRateNotSupported = "exchange_rate_not_supported";
    }
}
