using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Admin.Pages
{
    public class UserBalanceModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountService _accountService;
        private readonly ILogger _logger;

        public ApplicationUser CurrentUser { get; set; }
        public Dictionary<Currency, decimal> Balances { get; set; }

        public UserBalanceModel(ILogger<UserBalancesModel> logger, UserManager<ApplicationUser> userManager, IAccountService accountService)
        {
            _logger = logger;
            _userManager = userManager;
            _accountService = accountService;
        }

        public async Task OnGetAsync(Guid id)
        {
            _logger.LogDebug("Loading user balance");
            CurrentUser = await _userManager.FindByIdAsync(id.ToString());
            Balances = await _accountService.GetPorfolio(id);
        }
    }
}