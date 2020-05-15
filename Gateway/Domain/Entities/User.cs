using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.Gateway.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public DateTime Registered { get; set; }
        public bool Confirmed { get; set; }
        public Guid? ReferralId { get; set; }
        public string ReferralCode { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }

    }
}
