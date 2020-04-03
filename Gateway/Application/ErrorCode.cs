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
        /// <summary>
        /// The instant mining operation is locked because of timeout.
        /// </summary>
        public const string MiningInstantTimeout = "mining_instant_timeout";
        /// <summary>
        /// The bonus mining operation is locked because of timeout.
        /// </summary>
        public const string MiningBonusTimeout = "mining_bonus_timeout";
        /// <summary>
        /// The mining request not supported for the user.
        /// </summary>
        public const string MiningRequestNotSupported = "mining_request_not_supported";
        /// <summary>
        /// The operation couldn't perform because the exchange rate pair is not supported.
        /// </summary>
        public const string ExchangeRateNotSupported = "exchange_rate_not_supported";
        /// <summary>
        /// The error of some operation locked because of insufficient minings amount.
        /// </summary>
        public const string InsufficientMinings = "insufficient_minings";
        /// <summary>
        /// The error of some operation locked because multi factor requirement.
        /// </summary>
        public const string MultiFactorRequired = "multi_factor_required";
        /// <summary>
        /// The operation couldn't perform because the user account is not confirmed.
        /// </summary>
        public const string ConfirmationRequired = "confirmation_required";
    }
}
