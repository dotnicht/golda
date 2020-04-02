using AutoMapper;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Application.Models;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Binebase.Exchange.Gateway.Infrastructure.Identity
{
    public class IdentityService : IIdentityService, IConfigurationProvider<IdentityService.Configuration>, ITransient<IIdentityService>
    {
        private readonly IDateTime _dateTime;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly Configuration _configuration;

        public IdentityService(
            IDateTime dateTime,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<Configuration> options)
            => (_userManager, _signInManager, _dateTime, _mapper, _configuration)
                = (userManager, signInManager, dateTime, mapper, options.Value);

        public async Task<User> GetUser(string userName)
            => _mapper.Map<User>(await _userManager.FindByNameAsync(userName));

        public async Task<User> GetUser(Guid userId)
            => _mapper.Map<User>(await _userManager.FindByIdAsync(userId.ToString()));

        public async Task<Result> CreateUser(Guid id, string userName, string password, string code)
        {
            var referral = _userManager.Users.SingleOrDefault(x => code != null && x.ReferralCode == code.Trim());

            var user = new ApplicationUser
            {
                Id = id,
                UserName = userName,
                Email = userName,
                Registered = _dateTime.UtcNow,
                ReferralCode = Regex.Replace(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), "[/+=]", string.Empty),
                ReferralId = referral?.Id
            };

            var result = await _userManager.CreateAsync(user, password);
            return result.ToApplicationResult();
        }

        public async Task<string> GenerateConfirmationUrl(Guid userId)
            => await Task.FromResult(string.Format(_configuration.ConfirmationUrlFormat, userId, HttpUtility.UrlEncode(await GenerateConfirmationToken(userId))));

        public async Task<string> GenerateResetPasswordUrl(Guid userId)
            => await Task.FromResult(string.Format(_configuration.ResetPasswordUrlFormat, userId, HttpUtility.UrlEncode(await GeneratePasswordResetToken(userId))));

        public async Task<string> GenerateAuthenticatorUrl(User user, string key)
            => await Task.FromResult(string.Format(_configuration.AuthenticatorUrlFormat, key, HttpUtility.UrlEncode(user.Email)));

        public Task<string> GenerateConfirmationToken(Guid userId)
            => _userManager.GenerateEmailConfirmationTokenAsync(_userManager.Users.Single(u => u.Id == userId));

        public Task<string> GeneratePasswordResetToken(Guid userId)
            => _userManager.GeneratePasswordResetTokenAsync(_userManager.Users.Single(u => u.Id == userId));

        public async Task<Result> ResetPassword(Guid userId, string token, string newPassword)
        {
            var result = await _userManager.ResetPasswordAsync(_userManager.Users.Single(u => u.Id == userId), HttpUtility.UrlDecode(token), newPassword);
            return result.ToApplicationResult();
        }

        public async Task<(string Key, string Uri)> GetAuthenticatorData(User user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var key = await _userManager.GetAuthenticatorKeyAsync(_userManager.Users.Single(u => u.Id == user.Id));

            if (string.IsNullOrWhiteSpace(key))
            {
                await _userManager.ResetAuthenticatorKeyAsync(_userManager.Users.Single(u => u.Id == user.Id));
                key = await _userManager.GetAuthenticatorKeyAsync(_userManager.Users.Single(u => u.Id == user.Id));
            }

            var result = new StringBuilder();
            var index = 0;
            const int size = 4;

            while (index + size < key.Length)
            {
                result.Append(key.Substring(index, 4)).Append(" ");
                index += size;
            }

            if (index < key.Length)
            {
                result.Append(key.Substring(index));
            }

            return (result.ToString().ToLowerInvariant(), string.Format(_configuration.AuthenticatorUrlFormat, key, HttpUtility.UrlEncode(user.Email)));
        }

        public async Task<bool> GetTwoFactorEnabled(Guid userId) 
            => await _userManager.GetTwoFactorEnabledAsync(_userManager.Users.Single(u => u.Id == userId));

        public async Task<Result> SetTwoFactorAuthentication(Guid userId, bool isEnabled)
        {
            var result = await _userManager.SetTwoFactorEnabledAsync(_userManager.Users.Single(u => u.Id == userId), isEnabled);
            return result.ToApplicationResult();
        }

        public async Task<bool> VerifyTwoFactorToken(Guid userId, string token) 
            => await _userManager.VerifyTwoFactorTokenAsync(_userManager.Users.Single(u => u.Id == userId), _userManager.Options.Tokens.AuthenticatorTokenProvider, token);

        public async Task<bool> CheckUserPassword(Guid userId, string password) 
            => await _userManager.CheckPasswordAsync(_userManager.Users.Single(u => u.Id == userId), password);

        public async Task<string> GenerateAuthToken(User user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.AuthSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return await Task.FromResult(tokenHandler.WriteToken(token));
        }

        public async Task<Result> ConfirmToken(Guid userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var result = await _userManager.ConfirmEmailAsync(user, HttpUtility.UrlDecode(code));
            return result.ToApplicationResult();
        }

        public async Task<Result> Authenticate(User user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var app = await _userManager.FindByNameAsync(user.Email);
            if (!await _signInManager.CanSignInAsync(app))
            {
                return Result.Failure("signin_error");
            }

            await _signInManager.SignInAsync(app, true);
            return Result.Success();
        }

        public class Configuration
        {
            public string AuthSecret { get; set; }
            public string ConfirmationUrlFormat { get; set; }
            public string ResetPasswordUrlFormat { get; set; }
            public string AuthenticatorUrlFormat { get; set; }
        }
    }
}
