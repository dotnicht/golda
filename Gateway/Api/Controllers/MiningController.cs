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
    public class MiningController : ApiController
    {
        [HttpGet, ProducesResponseType(typeof(MiningStatusQueryResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<MiningStatusQueryResult>> Status([FromQuery]MiningStatusQuery query) 
            => await Mediator.Send(query);

        [HttpPost, ProducesResponseType(typeof(MiningBonusCommandResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<MiningBonusCommandResult>> Bonus(MiningBonusCommand command) 
            => await Mediator.Send(command);

        [HttpPost, ProducesResponseType(typeof(MiningInstantCommandResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<MiningInstantCommandResult>> Instant(MiningInstantCommand command) 
            => await Mediator.Send(command);

        [HttpPost, AllowAnonymous, ProducesResponseType(typeof(RequestMiningCommandResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<RequestMiningCommandResult>> RequestMining(RequestMiningCommand command) 
            => await Mediator.Send(command);

        [HttpPost, ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Promotion(ExchangePromotionCommand command) 
            => Convert(await Mediator.Send(command));
    }
}