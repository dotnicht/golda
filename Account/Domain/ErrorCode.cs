namespace Binebase.Exchange.AccountService.Domain
{
    public static class ErrorCode
    {
        public const string AccountNotExists = "account_not_exists";
        public const string AccountExists = "account_exists";
        public const string AccountLocked = "account_locked";
        public const string AccountUnlocked = "account_unlocked";
        public const string AssetNotExists = "asset_not_exists";
        public const string AssetExists = "asset_exists";
        public const string AssetLocked = "asset_locked";
        public const string AssetUnlocked = "asset_unlocked";
        public const string InsufficientBalance = "insufficient_balance";
        public const string NonZeroBalance = "non_zero_balance";
    }
}
