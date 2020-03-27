using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.Gateway.Domain.Entities
{
    public class User
    {
        // TODO: extend user with referral info.
        public Guid Id { get; set; }
        public string Email { get; set; }
        public DateTime Registered { get; set; }
        public bool Confirmed { get; set; }
    }
}
