using Binebase.Exchange.AccountService.Domain.Common;
using Binebase.Exchange.Common.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.AccountService.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IDbContext
    {
        public ApplicationDbContext(
            DbContextOptions options) : base(options)
        {
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }
    }
}
