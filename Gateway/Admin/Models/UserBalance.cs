using Binebase.Exchange.Common.Domain;
using System;

namespace Binebase.Exchange.Gateway.Admin.Models
{
    public class UserBalance
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public Currency Currency { get; set; }
        public decimal Balance { get; set; }
    }
}
