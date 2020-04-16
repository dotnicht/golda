using Binebase.Exchange.Gateway.Admin.Helpers;
using Binebase.Exchange.Gateway.Admin.Models;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Admin.Pages
{
    public class UserBalancesModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountService _accountService;
        private ILogger _logger;

        public string CurrencySort { get; set; }
        public string UserNameSort { get; set; }
        public string BalanceSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public int PageSize { get; set; }
        public List<string> UsersNames { get; set; }
        public List<string> Currencies { get; set; }
        public PaginatedList<UserBalance> UserBalances { get; set; }
        public List<UserBalance> UserBalancesIQ { get; set; }

        public UserBalancesModel(ILogger<UserBalancesModel> logger, UserManager<ApplicationUser> userManager, IAccountService accountService)
        {
            _logger = logger;
            _userManager = userManager;
            _accountService = accountService;
            UserBalancesIQ = new List<UserBalance>();
            PageSize = 10;
        }
        public async Task OnGetAsync(string sortOrder, string currentFilterFieldName, string currentFilter, string searchString, int? pageIndex)
        {
            _logger.LogDebug("Loading users balances");

            CurrentSort = sortOrder;
            UserNameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            CurrencySort = sortOrder == "currency" ? "currency_desc" : "currency";
            BalanceSort = sortOrder == "Balance" ? "Balance_desc" : "Balance";

            #region Load Data
            var userIds = _userManager.Users.ToDictionary(u => u.Id, u => u.Email);

            foreach (var userId in userIds)
            {
                var userPortfolio = await _accountService.GetPorfolio(userId.Key);
                foreach (var item in userPortfolio)
                {
                    UserBalancesIQ.Add(new UserBalance
                    {
                        UserId = userId.Key,
                        Email = userId.Value,
                        Currency = item.Key,
                        Balance = item.Value
                    });
                }
            }
            #endregion

            #region filteringLists
            UsersNames = UserBalancesIQ.Select(t => t.Email).Distinct().ToList();
            Currencies = UserBalancesIQ.Select(t => t.Currency.ToString()).Distinct().ToList();
            #endregion

            #region Filtering logic

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else if (string.IsNullOrEmpty(currentFilterFieldName))
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            if (!string.IsNullOrEmpty(searchString) && string.IsNullOrEmpty(currentFilterFieldName))
            {
                UserBalancesIQ = UserBalancesIQ.Where(u => u.Email.Contains(searchString)
                                       || u.Email.Contains(searchString)).ToList();
            }

            if (!string.IsNullOrEmpty(currentFilter))
            {
                switch (currentFilterFieldName)
                {
                    case "Currency":
                        UserBalancesIQ = UserBalancesIQ.Where(t => t.Currency.ToString().Equals(currentFilter, System.StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                    case "UserName":
                        UserBalancesIQ = UserBalancesIQ.Where(t => t.Email.Equals(currentFilter, System.StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                    default:
                        break;
                }
            }
            #endregion

            #region Sorting logic
            switch (sortOrder)
            {
                case "name_desc":
                    UserBalancesIQ = UserBalancesIQ.OrderByDescending(u => u.Email).ToList();
                    break;
                case "currency":
                    UserBalancesIQ = UserBalancesIQ.OrderBy(u => u.Currency).ToList();
                    break;
                case "currency_desc":
                    UserBalancesIQ = UserBalancesIQ.OrderByDescending(u => u.Currency).ToList();
                    break;
                case "Balance":
                    UserBalancesIQ = UserBalancesIQ.OrderBy(u => u.Balance).ToList();
                    break;
                case "Balance_desc":
                    UserBalancesIQ = UserBalancesIQ.OrderByDescending(u => u.Balance).ToList();
                    break;
                default:
                    break;
            }
            #endregion         

            UserBalances = PaginatedList<UserBalance>.Create(UserBalancesIQ, pageIndex ?? 1, PageSize);
        }
    }
}