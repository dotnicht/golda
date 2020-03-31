using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Binebase.Exchange.Gateway.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Admin.Models;
using Admin.Helpers;

namespace Admin
{
    public class TransactionsModel : PageModel
    {
        private readonly ILogger<TransactionsModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountService _accountService;
        private List<Binebase.Exchange.Gateway.Domain.Entities.Transaction> _allTransactions;
        private IList<Guid> UserIDs { get; set; }
        private List<TransactionExt> _transactions;
        public Tab ActiveTab { get; set; }
        public string NameSort { get; set; }
        public string DateSort { get; set; }
        public string TwoFactorEnabledSort { get; set; }
        public string EmailConfirmedSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public int PageSize { get; set; }

        public List<string> Currencies { get; set; }
        public List<string> Sources { get; set; }
        public List<string> UserIds { get; set; }
        public List<string> Types { get; set; }

        public List<TransactionExt> TransactionsIQ { get; set; }
        public PaginatedList<TransactionExt> Transactions { get; set; }
        public TransactionsModel(ILogger<TransactionsModel> logger, UserManager<ApplicationUser> userManager, IAccountService accountService)
        {
            _logger = logger;
            _userManager = userManager;
            _accountService = accountService;
            _allTransactions = new List<Binebase.Exchange.Gateway.Domain.Entities.Transaction>();
            _transactions = new List<TransactionExt>();
            PageSize = 10;
        }

        public async Task OnGet(Tab aTab, string sortOrder, string currentFilterFieldName, string currentFilter, string searchString, int? pageIndex)
        {
            ActiveTab = aTab;

            UserIDs = (from u in _userManager.Users select u.Id).ToList();
            foreach (var id in UserIDs)
            {
                var trans = await _accountService.GetTransactions(id);
                _allTransactions.AddRange(trans);
                _transactions.AddRange(MapingHelper.MapToTransactionExt(trans, id));

            }

            CurrentSort = sortOrder;
            NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";
            TwoFactorEnabledSort = sortOrder == "TwoFactorEnabled" ? "TwoFactorDisabled" : "TwoFactorEnabled";
            EmailConfirmedSort = sortOrder == "EmailConfirmed" ? "EmailNotConfirmed" : "EmailConfirmed";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            CurrentFilter = searchString;

            TransactionsIQ = _transactions.Where(t => t.Type == (ActiveTab == Tab.Deposits ?
                                                                   Binebase.Exchange.Common.Domain.TransactionType.Deposit :
                                                                   Binebase.Exchange.Common.Domain.TransactionType.Widthraw)).ToList();
            #region Filtering
            if (!string.IsNullOrEmpty(currentFilter))
            {
                switch (currentFilterFieldName)
                {
                    case "Currency":
                        TransactionsIQ = TransactionsIQ.Where(t => t.Currency.ToString().Contains(searchString)).ToList();
                        break;
                    case "Source":
                        TransactionsIQ = TransactionsIQ.Where(t => t.Type.ToString().Contains(searchString)).ToList();
                        break;
                    case "Amount":
                        TransactionsIQ = TransactionsIQ.Where(t => t.Amount.ToString().Contains(searchString)).ToList();
                        break;
                    case "Balance":
                        TransactionsIQ = TransactionsIQ.Where(t => t.Balance.ToString().Contains(searchString)).ToList();
                        break;
                    case "UserId":
                        TransactionsIQ = TransactionsIQ.Where(t => t.UserId.ToString().Contains(searchString)).ToList();
                        break;
                    case "Type":
                        TransactionsIQ = TransactionsIQ.Where(t => t.Type.ToString().Contains(searchString)).ToList();
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
                    TransactionsIQ = TransactionsIQ.OrderBy(u => u.DateTime).ToList();
                    break;
                case "date_desc":
                    TransactionsIQ = TransactionsIQ.OrderByDescending(u => u.DateTime).ToList();
                    break;
                default:
                    TransactionsIQ = TransactionsIQ.OrderBy(u => u.UserId).ToList();
                    break;
            }
            #endregion

            #region filteringLists
            Currencies = TransactionsIQ.Select(t => t.Currency.ToString()).Distinct().ToList();
            Sources = TransactionsIQ.Select(t => t.Type.ToString()).Distinct().ToList();
            UserIds = TransactionsIQ.Select(t => t.UserId.ToString()).Distinct().ToList();
            Types = TransactionsIQ.Select(t => t.Type.ToString()).Distinct().ToList();
            #endregion

            Transactions = PaginatedList<TransactionExt>.Create(TransactionsIQ, pageIndex ?? 1, PageSize);
        }

        public async Task OnPostSwitchToTabsAsync(string tabName)
        {
            switch (tabName)
            {
                case "Deposits":
                    ActiveTab = Tab.Deposits;
                    break;
                case "Withdraw":
                    ActiveTab = Tab.Withdraw;
                    break;
                default:
                    ActiveTab = Tab.Deposits;
                    break;
            }
            await OnGet(ActiveTab, "", "", "", "", null);
        }
    }
}