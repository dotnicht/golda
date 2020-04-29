using Binebase.Exchange.AccountService.Application.Queries;
using Binebase.Exchange.AccountService.Contracts.Commands;
using Binebase.Exchange.Common.Api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Binebase.Exchange.AccountService.Api.Controllers
{
    public class AccountController : ApiController
    {
        [HttpGet, ProducesResponseType(typeof(PortfolioQueryResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<PortfolioQueryResult>> Portfolio([FromQuery] PortfolioQuery query) => await Mediator.Send(query);

        [HttpGet, ProducesResponseType(typeof(TransactionsQueryResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<TransactionsQueryResult>> Transactions([FromQuery]TransactionsQuery query) => await Mediator.Send(query);

        [HttpPost("/api/[controller]"), ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> New(NewAccountCommand command) => Convert(await Mediator.Send(command));

        [HttpPost, ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Lock(LockAccountCommand command) => Convert(await Mediator.Send(command));

        [HttpPost, ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Unlock(UnlockAccountCommand command) => Convert(await Mediator.Send(command));
    }
}
