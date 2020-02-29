using Binebase.Exchange.CryptoService.Domain.Common;
using Binebase.Exchange.CryptoService.Domain.Enums;
using System;

namespace Binebase.Exchange.CryptoService.Domain.Entities
{
    public class TodoItem : AuditableEntity
    {
        public long Id { get; set; }

        public int ListId { get; set; }

        public string Title { get; set; }

        public string Note { get; set; }

        public bool Done { get; set; }

        public DateTime? Reminder { get; set; }

        public PriorityLevel Priority { get; set; }


        public TodoList List { get; set; }
    }
}
