using System;

namespace Binebase.Exchange.CryptoService.Application.Commands
{
    public class PublishTransactionCommandResult
    {
        public Guid Id { get; set; }
        public string Hash { get; set; }
    }
}
