using Binebase.Exchange.Gateway.Infrastructure.Entities;
using Binebase.Exchange.Gateway.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace Binebase.Exchange.Gateway.Infrastructure.Interfaces
{
    public interface IUserContext : IDisposable
    {
        DbSet<ApplicationUser> Users { get; set; }
        DbSet<BalanceConsistencyRecord> BalanceRecords { get; set; }
    }
}
