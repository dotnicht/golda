using Binebase.Exchange.Common.Application.Models;
using Binebase.Exchange.Gateway.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<Result> CreateUser(Guid id, string userName, string password, string code);
        Task<int> GetReferrersCount(Guid id);
        Task<User> GetUser(string userName);
        Task<User> GetUser(Guid userId);
        Task<Result> Authenticate(User user);
        Task<string> GenerateConfirmationUrl(Guid userId);
        Task<string> GenerateResetPasswordUrl(Guid userId);
        Task<string> GenerateAuthToken(User user);
        Task<Result> ResetPassword(Guid userId, string token, string newPassword);
        Task<Result> ConfirmToken(Guid userId, string code);
        Task<(string Key, string Uri)> GetAuthenticatorData(User user);
        Task<bool> GetTwoFactorEnabled(Guid userId);
        Task<Result> SetTwoFactorAuthentication(Guid userId, bool isEnabled);
        Task<bool> VerifyTwoFactorToken(Guid userId, string token);
        Task<bool> CheckUserPassword(Guid userId, string password);
        Task<Result> SetPhoneNumberVerify(Guid userId, bool isEnabled);
        Task<Result> PreSignInCheck(User user);
        Task<Result> UpdateUserPhoneNumber(Guid userId, string phoneNumber);
        bool CheckPhoneNumberForUniqueness(string phoneNumber);
    }
}
