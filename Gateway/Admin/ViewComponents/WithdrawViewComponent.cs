using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Admin.Models;
using System.Linq;

namespace Admin.ViewComponents
{
    public class WithdrawViewComponent : ViewComponent
    {
        private List<TransactionExt> _transactions;
        public WithdrawViewComponent()
        {
        }
        public IViewComponentResult Invoke(List<TransactionExt> transactions)
        {
            _transactions = transactions.Where(x => x.Source == Binebase.Exchange.Gateway.Domain.Enums.TransactionSource.Widthraw).ToList();
            return View(_transactions);
        }
    }
}
