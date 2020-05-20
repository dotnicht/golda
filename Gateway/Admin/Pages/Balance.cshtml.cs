using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Binebase.Exchange.Gateway.Admin.Pages
{
    public class BalanceModel : PageModel
    {
        private readonly ILogger<BalanceModel> _logger;
        private readonly IApplicationDbContext _dbContext;

        public BalanceModel(ILogger<BalanceModel> logger, IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public void OnGet()
        {
            _logger.LogDebug("Loading balances");

        }
    }
}
