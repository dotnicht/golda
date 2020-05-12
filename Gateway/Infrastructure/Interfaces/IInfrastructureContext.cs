using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Gateway.Infrastructure.Entities;
using Binebase.Exchange.Gateway.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace Binebase.Exchange.Gateway.Infrastructure.Interfaces
{
    public interface IInfrastructureContext : IDbContext
    {
        DbSet<ApplicationUser> Users { get; set; }
        DbSet<BalanceConsistencyRecord> BalanceRecords { get; set; }
    }
}
