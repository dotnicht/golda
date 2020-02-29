using Binebase.Exchange.AccountService.Application;
using Binebase.Exchange.AccountService.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Binebase.Exchange.AccountService.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        public Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public Task<Result> DeleteUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserNameAsync(string userId)
        {
            return Task.FromResult(userId);
        }
    }
}
