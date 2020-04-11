using Binebase.Exchange.Common.Api.Controllers;
using Binebase.Exchange.CryptoService.Application.Commands;
using Binebase.Exchange.CryptoService.Application.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Api.Controllers
{
    public class CryptoController : ApiController
    {
        [HttpPost, ProducesResponseType(typeof(GenerateAddressCommandResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<GenerateAddressCommandResult>> Addresses(GenerateAddressCommand command)
            => await Mediator.Send(command);

        [HttpGet, ProducesResponseType(typeof(AddressesQueryResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<AddressesQueryResult>> Addresses([FromQuery]AddressesQuery query)
            => await Mediator.Send(query);

        [HttpPost, ProducesResponseType(typeof(PublishTransactionCommandResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<PublishTransactionCommandResult>> Transactions(PublishTransactionCommand command)
            => await Mediator.Send(command);

        [HttpGet, ProducesResponseType(typeof(TransactionsQueryResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<TransactionsQueryResult>> Transactions([FromQuery]TransactionsQuery query)
            => await Mediator.Send(query);
    }
}
