using System;

namespace Binebase.Exchange.Gateway.Domain
{
    public abstract class AuditableEntity : Common.Domain.AuditableEntity
    {
        public Guid CreatedBy { get; set; }
        public Guid LastModifiedBy { get; set; }
    }
}
