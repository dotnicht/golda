using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.CryptoService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Binebase.Exchange.CryptoService.Application.Interfaces
{
    public interface IApplicationDbContext : IDbContext
    {
        DbSet<Address> Addresses { get; set; }
        DbSet<Transaction> Transactions { get; set; }
    }
}
