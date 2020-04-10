using Binebase.Exchange.Gateway.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.Gateway.Worker
{
    public class SystemUserService : ICurrentUserService
    {
        // TODO: implement system user.
        public Guid UserId { get; }
        public bool IsAnonymous { get; }
    }
}
