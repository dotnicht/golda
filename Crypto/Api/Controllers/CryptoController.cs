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
        public async Task<ActionResult<GenerateAddressCommandResult>> Address(GenerateAddressCommand command)
            => await Mediator.Send(command);

        [HttpGet, ProducesResponseType(typeof(AddressesQueryResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<AddressesQueryResult>> Addresses([FromQuery]AddressesQuery quary)
            => await Mediator.Send(quary);

        [HttpPost, ProducesResponseType(typeof(PublishTransactionCommandResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<PublishTransactionCommandResult>> Publish(PublishTransactionCommand command)
            => await Mediator.Send(command);
    }
}
