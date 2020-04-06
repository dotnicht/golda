using Binebase.Exchange.Gateway.Infrastructure.Identity;
using System;


namespace Binebase.Exchange.Gateway.Admin.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public DateTime Registered { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool EmailConfirmed { get; set; }
    }
    public class EditUser : User
    {
        public ApplicationUser AppUser { get; set; }
        public EditUser(ApplicationUser applicationUser)
        {
            AppUser = applicationUser;
        }
        public EditUser()
        {
        }

        public bool IsLocked
        {
            get { return AppUser.LockoutEnd > DateTime.UtcNow; }
        }
    }
}
