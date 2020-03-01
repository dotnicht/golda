using AutoMapper;
using Binebase.Exchange.Common.Application.Mappings;
using Binebase.Exchange.Gateway.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Binebase.Exchange.Gateway.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>, IMapTo<User>
    {
        public DateTime Registered { get; set; }
        public string ReferralCode { get; set; }
        public Guid? ReferralId { get; set; }
        public ApplicationUser ReferralUser { get; set; }
        public ICollection<ApplicationUser> Refferals { get; } = new HashSet<ApplicationUser>();

        public void MappingTo(Profile profile)
            => profile.CreateMap<ApplicationUser, User>().ForMember(x => x.Confirmed, x => x.MapFrom(y => y.EmailConfirmed));
    }
}