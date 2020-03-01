using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Persistence
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext, IDbContext, IScoped<IDbContext>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;

        public DbSet<MiningRequest> MiningRequests { get; set; }
        public DbSet<Promotion> Promotions { get; set; }

        public DbContext(DbContextOptions<DbContext> options, ICurrentUserService currentUserService, IDateTime dateTime)
            : base(options)
              => (_currentUserService, _dateTime) = (currentUserService, dateTime);

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = _currentUserService.UserId;
                        entry.Entity.Created = _dateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = _currentUserService.UserId;
                        entry.Entity.LastModified = _dateTime.UtcNow;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }
    }
}
