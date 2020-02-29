using Binebase.Exchange.Gateway.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Infrastructure.Persistence
{
    public static class IdentityDbContextSeed
    {
        private static readonly string[] _roles = new[] { "User", "Support", "Administrator" };

        public static async Task SeedAsync(IdentityDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            //_roles.ToList().ForEach(async x => await roleManager.CreateAsync(new IdentityRole<Guid> { Name = x, NormalizedName = x.ToUpper() }));
            await Task.CompletedTask;
        }
    }
}
