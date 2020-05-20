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
        /// The exchange operation is not valid, e.g. both base and quote amounts exist.
        /// </summary>
        public const string ExchangeOperationInvalid = "exchange_operation_invalid";
        /// <summary>
        /// The error of some operation locked because of insufficient minings amount.
        /// </summary>
        public const string InsufficientMinings = "insufficient_minings";
        /// <summary>
        /// The error of some operation locked because of insufficient balance.
        /// </summary>
        public const string InsufficientBalance = "insufficient_balance";
        /// <summary>
        /// The boost value is not supported.
        /// </summary>
        public const string UnsupportedBoost = "unsupported_boost";
        /// <summary>
        /// The error of some operation locked because multi factor requirement.
        /// </summary>
        public const string MultiFactorRequired = "multi_factor_required";
        /// <summary>
        /// The operation couldn't perform because the user account is not confirmed.
        /// </summary>
        public const string ConfirmationRequired = "confirmation_required";
        /// <summary>
        /// The operation couldn't perform because the currency is not supported.
        /// </summary>
        public const string CurrencyNotSupported = "currency_not_supported";
        /// <summary>
        /// The withdraw limit has been reached.
        /// </summary>
        public const string WithdrawLimit = "withdraw_limit";
        /// <summary>
        /// The request didn't reach withdraw minimum requirement.
        /// </summary>
        public const string WithdrawMinimum = "withdraw_minimum";
        /// <summary>
        /// Phone number is empty or has invalid format
        /// </summary>
        public const string InvalidPhoneNumber = "invalid_phone_number";
        /// <summary>
        /// User cannot sign in without a confirmed phone number.
        /// </summary>
        public const string NotConfirmedPhoneNumber = "not_confirmed_phone_number";
        /// <summary>
        ///User cannot sign in without a confirmed email.
        /// </summary>
        public const string NotConfirmedEmail = "not_confirmed_email";
        /// <summary>
        /// User is locked out.
        /// </summary>
        public const string UserIsLocked = "user_is_locked";
        /// <summary>
        /// User cannot sign in without a confirmed email and phone number
        /// </summary>
        public const string NotConfirmedUser = "not_confirmed_user";
        /// <summary>
        /// Input phone number is already in use
        /// </summary>
        public const string PhoneNumberAlreadyInUse = "phone_number_already_in_use";
    }
}
