using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface IApplicationDbContext : IDbContext
    {
        DbSet<MiningRequest> MiningRequests { get; set; }
        DbSet<Promotion> Promotions { get; set; }
        DbSet<ExchangeOperation> ExchangeOperations { get; set; }
    }
}
