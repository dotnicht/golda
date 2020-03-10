using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Application.Interfaces
{
    public interface IBitcoinService
    {
        Task<string> GenerateKeys();
    }
}
