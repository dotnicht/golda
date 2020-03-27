using System;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        bool IsAnonymous => UserId == default;
    }
}
