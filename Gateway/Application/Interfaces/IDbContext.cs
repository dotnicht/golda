using Binebase.Exchange.Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface IDbContext : Common.Application.Interfaces.IDbContext
    {
        DbSet<MiningRequest> MiningRequests { get; set; }
        DbSet<Promotion> Promotions { get; set; }
    }
}
