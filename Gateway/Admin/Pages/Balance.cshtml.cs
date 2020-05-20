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
            PageSize = 20;
        }

        public void OnGet(string sortOrder, string currentFilterFieldName, string currentFilter, string searchString, int? pageIndex)
        {
            _logger.LogDebug("Loading balances");

            CurrentSort = sortOrder;
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";

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
                    case "Currency":
                        BalanceRecordsIQ = BalanceRecordsIQ.Where(b => b.Currency.ToString().Contains(searchString)).ToList();
                        break;
                    default:
                        break;
                }
            }
            #endregion

            #region Sorting
            switch (sortOrder)
            {
                case "Date":
                    BalanceRecordsIQ = BalanceRecordsIQ.OrderBy(u => u.Created).ToList();
                    break;
                case "date_desc":
                    BalanceRecordsIQ = BalanceRecordsIQ.OrderByDescending(u => u.Created).ToList();
                    break;
                default:
                    break;
            }
            #endregion

            #region filteringLists
            Currencies = BalanceRecordsIQ.Select(b => b.Currency.ToString()).Distinct().ToList();

            #endregion

            BalanceRecords = PaginatedList<BalanceConsistencyRecord>.Create(
                BalanceRecordsIQ, pageIndex ?? 1, PageSize);
        }
    }
}
