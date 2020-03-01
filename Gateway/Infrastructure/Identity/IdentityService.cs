using AutoMapper;
using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Common.Application.Models;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Binebase.Exchange.Gateway.Infrastructure.Identity
{
    public class IdentityService : IIdentityService, IConfigurationProvider<IdentityService.Configuration>, ITransient<IIdentityService>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDateTime _dateTime;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly Configuration _configuration;

        public IdentityService(
            IHttpContextAccessor httpContextAccessor,
            IDateTime dateTime,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<Configuration> options)
            => (_httpContextAccessor, _userManager, _signInManager, _dateTime, _mapper,  _configuration)
                = (httpContextAccessor, userManager, signInManager, dateTime,  mapper, options.Value);

        public async Task<string> GetUserName(Guid userId)
        {
            if (userId == default)
            {
                return "Anonymous";
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user?.UserName;
        }

        public async Task<User> GetUser(string userName)
            => MapToUser(await _userManager.FindByNameAsync(userName));

        public async Task<User> GetUser(Guid userId)
            => MapToUser(await _userManager.FindByIdAsync(userId.ToString()));

        public async Task<(Result Result, Guid UserId)> CreateUser(string userName, string password)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = userName,
                Registered = _dateTime.UtcNow,
                ReferralCode = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, password);
            return (result.ToApplicationResult(), user.Id);
        }

        public async Task<string> GenerateConfirmationUrl(Guid userId)
            => await Task.FromResult(string.Format(_configuration.ConfirmationUrlFormat, userId, HttpUtility.UrlEncode(await GenerateConfirmationToken(userId))));

        public async Task<string> GenerateResetPasswordUrl(Guid userId)
            => await Task.FromResult(string.Format(_configuration.ResetPasswordUrlFormat, userId, HttpUtility.UrlEncode(await GeneratePasswordResetToken(userId))));

        public Task<string> GenerateConfirmationToken(Guid userId)
            => _userManager.GenerateEmailConfirmationTokenAsync(_userManager.Users.Single(u => u.Id == userId));

        public Task<string> GeneratePasswordResetToken(Guid userId)
            => _userManager.GeneratePasswordResetTokenAsync(_userManager.Users.Single(u => u.Id == userId));

        public async Task<Result> ResetPassword(Guid userId, string token, string newPassword)
        {
            var result = await _userManager.ResetPasswordAsync(_userManager.Users.Single(u => u.Id == userId), HttpUtility.UrlDecode(token), newPassword);
            return result.ToApplicationResult();
        }

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

        public async Task<Result> Authenticate(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) throw new NotFoundException(nameof(ApplicationUser), userName);

            var result = await _signInManager.PasswordSignInAsync(user, password, true, true);
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
                return Result.Failure("Unable to sign in.");
            }

            await _signInManager.SignInAsync(app, true);
            return Result.Success();
        }

        private User MapToUser(ApplicationUser user)
            => user == null
                ? null
                : new User { Id = user.Id, Email = user.Email, Registered = user.Registered, Confirmed = user.EmailConfirmed };

        public class Configuration
        {
            public string AuthSecret { get; set; }
            public string ConfirmationUrlFormat { get; set; }
            public string ResetPasswordUrlFormat { get; set; }
        }
    }
}
