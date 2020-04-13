using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Application.Mappings;
using Binebase.Exchange.Gateway.Domain.Entities;
using System;

namespace Binebase.Exchange.Gateway.Application.Commands
{
    public class SignInCommandResult : IMapFrom<User>
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string ReferralCode { get; set; }
        public string Token { get; set; }
    }
}
