using Binebase.Exchange.Gateway.Application.Commands;
using Binebase.Exchange.Gateway.Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Api.Controllers
{
    [Authorize]
    public class MiningController : ApiController
    {
        [HttpGet]
        public async Task<ActionResult<MiningStatusQueryResult>> Status([FromQuery]MiningStatusQuery query) => await Mediator.Send(query);
        [HttpPost]
        public async Task<ActionResult<MiningDailyCommandResult>> Daily(MiningDailyCommand command) => await Mediator.Send(command);
        [HttpPost, AllowAnonymous]
        new public async Task<ActionResult<MiningRequestCommandResult>> Request(MiningRequestCommand command) => await Mediator.Send(command);
    }
}