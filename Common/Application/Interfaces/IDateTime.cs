using System;

namespace Binebase.Exchange.Common.Application.Interfaces
{
    public interface IDateTime
    {
        DateTime UtcNow { get; }
    }
}
