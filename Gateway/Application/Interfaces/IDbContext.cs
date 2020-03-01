using Binebase.Exchange.Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface IDbContext : Binebase.Exchange.Common.Application.Interfaces.IDbContext
    {
        DbSet<MiningRequest> MiningRequests { get; set; }
        DbSet<Promotion> Promotions { get; set; }
    }
}
