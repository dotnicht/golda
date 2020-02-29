using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading;

namespace Binebase.Exchange.Gateway.Api.Services
{
    public class CurrentUserService : ICurrentUserService, IScoped<ICurrentUserService>
    {
        private readonly Lazy<Guid> _userId;
        public Guid UserId => _userId.Value;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor) =>
            _userId = new Lazy<Guid>(
                () => Guid.TryParse(httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ? userId : default, 
                LazyThreadSafetyMode.ExecutionAndPublication);
    }
}
