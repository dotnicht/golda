using Binebase.Exchange.Common.Application.Models;
using Binebase.Exchange.Gateway.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<User> GetUser(string userName);
        Task<User> GetUser(Guid userId);
        Task<(Result Result, Guid UserId)> CreateUser(string userName, string password);
        Task<string> GenerateConfirmationUrl(Guid userId);
        Task<string> GenerateResetPasswordUrl(Guid userId);
        Task<string> GenerateAuthenticatorUrl(User user, string key);
        Task<string> GenerateConfirmationToken(Guid userId);
        Task<string> GeneratePasswordResetToken(Guid userId);
        Task<Result> ResetPassword(Guid userId, string token, string newPassword);
        Task<string> GenerateAuthToken(User user);
        Task<Result> ConfirmToken(Guid userId, string code);
        Task<Result> Authenticate(string userName, string password);
        Task<Result> Authenticate(User user);
        Task<string> GetAuthenticatorKey(Guid userId);
        Task<Result> ResetAuthenticatorKey(Guid userId);
        Task<bool> GetTwoFactorEnabled(Guid userId);
        Task<Result> SetTwoFactorAuthentication(Guid userId, bool isEnabled);
        Task<bool> VerifyTwoFactorToken(Guid userId, string token);
        Task<bool> CheckUserPassword(Guid userId, string password);
    }
}
