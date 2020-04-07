using Binebase.Exchange.Gateway.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.Gateway.Infrastructure.Interfaces
{
    public interface IUserContext : IDisposable
    {
        DbSet<ApplicationUser> Users { get; set; }
    }
}
