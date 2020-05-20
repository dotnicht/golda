using System.Collections.Generic;
using System.Linq;
using Binebase.Exchange.Gateway.Admin.Helpers;
using Binebase.Exchange.Gateway.Infrastructure.Entities;
using Binebase.Exchange.Gateway.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Binebase.Exchange.Gateway.Admin.Pages
{
    public class BalanceModel : PageModel
    {
        private readonly ILogger<BalanceModel> _logger;
        private readonly IInfrastructureContext _dbContext;

        public int PageSize { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public List<BalanceConsistencyRecord> BalanceRecordsIQ { get; set; }
        public PaginatedList<BalanceConsistencyRecord> BalanceRecords { get; set; }

        public BalanceModel(ILogger<BalanceModel> logger, IInfrastructureContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
            BalanceRecordsIQ = new List<BalanceConsistencyRecord>();
            PageSize = 20;
        }

        public void OnGet(string sortOrder, string currentFilterFieldName, string currentFilter, string searchString, int? pageIndex)
        {
            _logger.LogDebug("Loading balances");

            CurrentSort = sortOrder;
            //NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            //DateSort = sortOrder == "Date" ? "date_desc" : "Date";
            //TwoFactorEnabledSort = sortOrder == "TwoFactorEnabled" ? "TwoFactorDisabled" : "TwoFactorEnabled";
            //EmailConfirmedSort = sortOrder == "EmailConfirmed" ? "EmailNotConfirmed" : "EmailConfirmed";
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else if (string.IsNullOrEmpty(currentFilterFieldName))
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;


            BalanceRecordsIQ = _dbContext.BalanceRecords.ToList();

            if (!string.IsNullOrEmpty(searchString) && string.IsNullOrEmpty(currentFilterFieldName))
            {
                BalanceRecordsIQ = BalanceRecordsIQ.Where(b => b.Currency.ToString().Contains(searchString)).ToList();
            }

            #region Filtering
            if (!string.IsNullOrEmpty(currentFilter))
            {
                switch (currentFilterFieldName)
                {
                    case "TwoFactorEnabled":
                        //UsersIQ = UsersIQ.Where(t => t.TwoFactorEnabled.ToString().Equals(currentFilter, System.StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                    case "EmailConfirmed":
                        //UsersIQ = UsersIQ.Where(t => t.EmailConfirmed.ToString().Equals(currentFilter, System.StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                    default:
                        break;
                }
            }
            #endregion

            #region Sorting
            switch (sortOrder)
            {
                //case "name_desc":
                //    UsersIQ = UsersIQ.OrderByDescending(u => u.UserName).ToList();
                //    break;
                //case "Date":
                //    UsersIQ = UsersIQ.OrderBy(u => u.Registered).ToList();
                //    break;
                //case "date_desc":
                //    UsersIQ = UsersIQ.OrderByDescending(u => u.Registered).ToList();
                //    break;
                //case "TwoFactorEnabled":
                //    UsersIQ = UsersIQ.OrderBy(u => u.TwoFactorEnabled).ToList();
                //    break;
                //case "TwoFactorDisabled":
                //    UsersIQ = UsersIQ.OrderByDescending(u => u.TwoFactorEnabled).ToList();
                //    break;
                //case "EmailConfirmed":
                //    UsersIQ = UsersIQ.OrderBy(u => u.EmailConfirmed).ToList();
                //    break;
                //case "EmailNotConfirmed":
                //    UsersIQ = UsersIQ.OrderByDescending(u => u.EmailConfirmed).ToList();
                //    break;

                default:
                    break;
            }
            #endregion

            #region filteringLists
            //UsersNames = UsersIQ.Select(t => t.UserName.ToString()).Distinct().ToList();

            #endregion

            BalanceRecords = PaginatedList<BalanceConsistencyRecord>.Create(
                BalanceRecordsIQ, pageIndex ?? 1, PageSize);
        }
    }
}
