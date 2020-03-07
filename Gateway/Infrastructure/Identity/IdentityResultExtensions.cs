using Binebase.Exchange.Common.Application.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;

namespace Binebase.Exchange.Gateway.Infrastructure.Identity
{
    public static class IdentityResultExtensions
    {
        public static Result ToApplicationResult(this IdentityResult result)
            => result.Succeeded
                ? Result.Success()
                : Result.Failure(result.Errors.Select(e => e.Description));

        public static Result ToApplicationResult(this SignInResult result)
            => result.Succeeded
                ? Result.Success()
                : Result.Failure(result.ToString());
    }
}