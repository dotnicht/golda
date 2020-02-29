using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.AccountService.Domain.Common
{
    public interface IIdContainer
    {
        Guid Id { get; set; }
    }
}
