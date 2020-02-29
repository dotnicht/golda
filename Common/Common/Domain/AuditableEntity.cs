using System;

namespace Binebase.Exchange.Common.Domain
{
    public abstract class AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public Guid LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
