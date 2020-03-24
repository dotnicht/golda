using Binebase.Exchange.Common.Api.Controllers;
using Binebase.Exchange.Gateway.Application.Commands;
using Binebase.Exchange.Gateway.Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Api.Controllers
{
    [Authorize]
    public class CryptoController : ApiController
    {
        [HttpGet, ProducesResponseType(typeof(AddressQueryResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<AddressQueryResult>> Address([FromQuery]AddressQuery query) 
            => await Mediator.Send(query);

        [HttpGet, ProducesResponseType(typeof(AddressesQueryResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<AddressesQueryResult>> Addresses([FromQuery]AddressesQuery query) 
            => await Mediator.Send(query ?? new AddressesQuery());

        [HttpGet, AllowAnonymous, ProducesResponseType(typeof(ExchangeRateQueryResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<ExchangeRateQueryResult>> ExchnageRate([FromQuery]ExchangeRateQuery query) 
            => await Mediator.Send(query);

        [HttpGet, AllowAnonymous, ProducesResponseType(typeof(ExchangeRatesQueryResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<ExchangeRatesQueryResult>> ExchnageRates([FromQuery]ExchangeRatesQuery query) 
            => await Mediator.Send(query ?? new ExchangeRatesQuery());

        [HttpPost, ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Exchange(ExchangeCommand command) 
            => Convert(await Mediator.Send(command));
    }
}
