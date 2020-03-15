using Binebase.Exchange.Common.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Binebase.Exchange.Common.Application.Interfaces
{
    public interface ITask
    {
        Task<Result> Execute();
    }
}
