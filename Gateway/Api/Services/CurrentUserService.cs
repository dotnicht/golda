using Binebase.Exchange.Gateway.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading;

namespace Binebase.Exchange.Gateway.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly Lazy<Guid?> _userId;

        public Guid UserId => _userId.Value ?? throw new InvalidOperationException("User not available.");

        public bool IsAnonymous => _userId.Value == default;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor) =>
            _userId = new Lazy<Guid?>(
                () => Guid.TryParse(httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
                    ? userId
                    : default(Guid?),
                LazyThreadSafetyMode.ExecutionAndPublication);
    }
}
