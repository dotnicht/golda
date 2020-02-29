using Binebase.Exchange.CryptoService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
       Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
