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

        public string CurrencySort { get; set; }
        public string DateSort { get; set; }
        public int PageSize { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public List<string> Currencies { get; set; }
        public List<BalanceConsistencyRecord> BalanceRecordsIQ { get; set; }
        public PaginatedList<BalanceConsistencyRecord> BalanceRecords { get; set; }

        public BalanceModel(ILogger<BalanceModel> logger, IInfrastructureContext dbContext)
        {
            _dbContext = dbContext;
            _logger = logger;
            BalanceRecordsIQ = new List<BalanceConsistencyRecord>();
            PageSize = 10;
        }

        public void OnGet(string sortOrder, string currentFilterFieldName, string currentFilter, string searchString, int? pageIndex)
        {
            _logger.LogDebug("Loading balances");

            CurrentFilter = currentFilter;
            CurrentSort = sortOrder;
            DateSort = sortOrder == "date" ? "date_desc" : "date";
            CurrencySort = sortOrder == "currency" ? "currency_desc" : "currency";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else if (string.IsNullOrEmpty(currentFilterFieldName))
            {
                searchString = currentFilter;
            }

            BalanceRecordsIQ = _dbContext.BalanceRecords.ToList();

            #region filteringLists
            Currencies = BalanceRecordsIQ.Select(b => b.Currency.ToString()).Distinct().ToList();
            #endregion

            if (!string.IsNullOrEmpty(searchString) && string.IsNullOrEmpty(currentFilterFieldName))
            {
                BalanceRecordsIQ = BalanceRecordsIQ.Where(b => b.Currency.ToString().Contains(searchString)).ToList();
            }

            #region Filtering
            if (!string.IsNullOrEmpty(currentFilter))
            {
                switch (currentFilterFieldName)
                {
                    case "Currency":
                        BalanceRecordsIQ = BalanceRecordsIQ.Where(b => b.Currency.ToString().Contains(currentFilter)).ToList();
                        break;
                    default:
                        break;
                }
            }
            #endregion

            #region Sorting
            switch (sortOrder)
            {
                case "date":
                    BalanceRecordsIQ = BalanceRecordsIQ.OrderBy(u => u.Created).ToList();
                    break;
                case "date_desc":
                    BalanceRecordsIQ = BalanceRecordsIQ.OrderByDescending(u => u.Created).ToList();
                    break;
                case "currency":
                    BalanceRecordsIQ = BalanceRecordsIQ.OrderBy(u => u.Currency).ToList();
                    break;
                case "currency_desc":
                    BalanceRecordsIQ = BalanceRecordsIQ.OrderByDescending(u => u.Currency).ToList();
                    break;
                default:
                    break;
            }
            #endregion


            BalanceRecords = PaginatedList<BalanceConsistencyRecord>.Create(
                BalanceRecordsIQ, pageIndex ?? 1, PageSize);
        }
    }
}
