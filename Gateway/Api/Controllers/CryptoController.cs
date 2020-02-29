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
        [HttpGet]
        public async Task<ActionResult<AddressQueryResult>> Address([FromQuery]AddressQuery query) => await Mediator.Send(query);
        [HttpGet]
        public async Task<ActionResult<AddressesQueryResult>> Addresses([FromQuery]AddressesQuery query) => await Mediator.Send(query);
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult<ExchangeRateQueryResult>> ExchnageRate([FromQuery]ExchangeRateQuery query) => await Mediator.Send(query);
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult<ExchangeRatesQueryResult>> ExchnageRates([FromQuery]ExchangeRatesQuery query) => await Mediator.Send(query);
        [HttpPost, ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Exchange(ExchangeCommand command) => Convert(await Mediator.Send(command));
    }
}
