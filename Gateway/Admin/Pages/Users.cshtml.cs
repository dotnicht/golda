using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Admin.Helpers;
using Admin.Models;
using Binebase.Exchange.Gateway.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Admin.Pages
{
    public class UsersModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public string NameSort { get; set; }
        public string DateSort { get; set; }
        public string TwoFactorEnabledSort { get; set; }
        public string EmailConfirmedSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public int PageSize { get; set; }
        public List<User>  UsersIQ { get; set; }
        public PaginatedList<User> Users { get; set; }

        public List<string> UsersNames { get; set; }


        public UsersModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            PageSize = 10;
        }

        public async Task OnGetAsync(string sortOrder, string currentFilterFieldName, string currentFilter, string searchString, int? pageIndex)
        {
            await Task.CompletedTask;

            CurrentSort = sortOrder;
            NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            DateSort = sortOrder == "Date" ? "date_desc" : "Date";
            TwoFactorEnabledSort = sortOrder == "TwoFactorEnabled" ? "TwoFactorDisabled" : "TwoFactorEnabled";
            EmailConfirmedSort = sortOrder == "EmailConfirmed" ? "EmailNotConfirmed" : "EmailConfirmed";
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else if(string.IsNullOrEmpty(currentFilterFieldName))
            {
                searchString = currentFilter;
            }

            CurrentFilter = searchString;

            UsersIQ = (from u in _userManager.Users
                                       select new User
                                       {
                                           Id = u.Id,
                                           UserName = u.UserName,
                                           Registered = u.Registered,
                                           PhoneNumber = u.PhoneNumber,
                                           TwoFactorEnabled = u.TwoFactorEnabled,
                                           EmailConfirmed = u.EmailConfirmed
                                       }).ToList();

            if (!string.IsNullOrEmpty(searchString)  &&  string.IsNullOrEmpty(currentFilterFieldName))
            {
                UsersIQ = UsersIQ.Where(u => u.UserName.Contains(searchString)
                                       || u.UserName.Contains(searchString)).ToList();
            }

            #region Filtering
            if (!string.IsNullOrEmpty(currentFilter))
            {
                switch (currentFilterFieldName)
                {
                    case "TwoFactorEnabled":
                        UsersIQ = UsersIQ.Where(t => t.TwoFactorEnabled.ToString().Equals(currentFilter, System.StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                    case "EmailConfirmed":
                        UsersIQ = UsersIQ.Where(t => t.EmailConfirmed.ToString().Equals(currentFilter, System.StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                    default:
                        break;
                }
            }
            #endregion

            #region Sorting
            switch (sortOrder)
            {
                case "name_desc":
                    UsersIQ = UsersIQ.OrderByDescending(u => u.UserName).ToList();
                    break;
                case "Date":
                    UsersIQ = UsersIQ.OrderBy(u => u.Registered).ToList();
                    break;
                case "date_desc":
                    UsersIQ = UsersIQ.OrderByDescending(u => u.Registered).ToList();
                    break;
                case "TwoFactorEnabled":
                    UsersIQ = UsersIQ.OrderBy(u => u.TwoFactorEnabled).ToList();
                    break;
                case "TwoFactorDisabled":
                    UsersIQ = UsersIQ.OrderByDescending(u => u.TwoFactorEnabled).ToList();
                    break;
                case "EmailConfirmed":
                    UsersIQ = UsersIQ.OrderBy(u => u.EmailConfirmed).ToList();
                    break;
                case "EmailNotConfirmed":
                    UsersIQ = UsersIQ.OrderByDescending(u => u.EmailConfirmed).ToList();
                    break;

                default:
                    break;
            }
            #endregion

            #region filteringLists
            UsersNames = UsersIQ.Select(t => t.UserName.ToString()).Distinct().ToList();

            #endregion

            Users = PaginatedList<User>.Create(
                UsersIQ, pageIndex ?? 1, PageSize);
        }
    }
}
