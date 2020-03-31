using Admin.Helpers;
using Admin.Models;
using Binebase.Exchange.Gateway.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.ViewComponents
{
    public class DepositsViewComponent : ViewComponent
    {
        public List<TransactionExt> _transactions;
        private readonly ILogger<DepositsViewComponent> _logger;

        public string NameSort { get; set; }
        public string DateSort { get; set; }
        public string TwoFactorEnabledSort { get; set; }
        public string EmailConfirmedSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public int PageSize { get; set; }
        public List<TransactionExt> TransactionsIQ { get; set; }
        public PaginatedList<TransactionExt> Transactions { get; set; }
        //public List<TransactionExt> Transactions { get; set; }
        public DepositsViewComponent(ILogger<DepositsViewComponent> logger)
        {
            _logger = logger;
            PageSize = 3;
        }
        public IViewComponentResult Invoke(List<TransactionExt> transactions)
        {
            _transactions = transactions;
            //Transactions = transactions.Where(x => x.Source == Binebase.Exchange.Gateway.Domain.Enums.TransactionSource.Deposit).ToList();
           // SomeMethod("", "", "", null);
            return View(this);
        }

        public void SomeMethod(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
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

            TransactionsIQ = _transactions.Where(t => t.Type == Binebase.Exchange.Common.Domain.TransactionType.Deposit).ToList();

            //if (!string.IsNullOrEmpty(searchString))
            //{
            //    TransactionsIQ = TransactionsIQ.Where(u => u..Contains(searchString)
            //                           || u.UserName.Contains(searchString));
            //}

            switch (sortOrder)
            {
                //case "name_desc":
                //    UsersIQ = UsersIQ.OrderByDescending(u => u.UserName);
                //    break;
                //case "Date":
                //    UsersIQ = UsersIQ.OrderBy(u => u.Registered);
                //    break;
                //case "date_desc":
                //    UsersIQ = UsersIQ.OrderByDescending(u => u.Registered);
                //    break;
                //case "TwoFactorEnabled":
                //    UsersIQ = UsersIQ.OrderBy(u => u.TwoFactorEnabled);
                //    break;
                //case "TwoFactorDisabled":
                //    UsersIQ = UsersIQ.OrderByDescending(u => u.TwoFactorEnabled);
                //    break;
                //case "EmailConfirmed":
                //    UsersIQ = UsersIQ.OrderBy(u => u.EmailConfirmed);
                //    break;
                //case "EmailNotConfirmed":
                //    UsersIQ = UsersIQ.OrderByDescending(u => u.EmailConfirmed);
                //    break;

                default:
                    TransactionsIQ = TransactionsIQ.OrderBy(t => t.Type).ToList();
                    break;
            }

            Transactions = PaginatedList<TransactionExt>.Create(TransactionsIQ, pageIndex ?? 1, PageSize);
        }
    }
}
