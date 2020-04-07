using System;
using System.Threading.Tasks;
using Binebase.Exchange.Gateway.Admin.Helpers;
using Binebase.Exchange.Gateway.Admin.Models;
using Binebase.Exchange.Gateway.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Binebase.Exchange.Gateway.Admin.Pages
{
    public class EditUserModel : PageModel
    {
        private readonly ILogger<UsersModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        public EditUser CurrentUser { get; set; }
        public EditUserModel(ILogger<UsersModel> logger, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }
        public async Task OnGetAsync(Guid id)
        {
            CurrentUser = MapingHelper.MapToEditUser(await _userManager.FindByIdAsync(id.ToString()));
        }

        public async Task<IActionResult> OnPostUpdateLockAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user.LockoutEnd > DateTime.UtcNow)
                user.LockoutEnd = DateTime.UtcNow;
            else
                user.LockoutEnd = DateTime.MaxValue;

            await _userManager.UpdateAsync(user);

            return RedirectToPage("EditUser", "OnGetAsync");

        }

    }
}