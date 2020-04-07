using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface ITransactionsSyncService
    {
        Task SyncTransactions();
    }
}
