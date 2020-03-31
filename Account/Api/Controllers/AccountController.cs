using Binebase.Exchange.AccountService.Application.Commands;
using Binebase.Exchange.AccountService.Application.Queries;
using Binebase.Exchange.Common.Api.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Binebase.Exchange.AccountService.Api.Controllers
{
    public class AccountController : ApiController
    {
        [HttpGet, ProducesResponseType(typeof(PortfolioQueryResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<PortfolioQueryResult>> Portfolio([FromQuery]PortfolioQuery query) => await Mediator.Send(query);

        [HttpGet, ProducesResponseType(typeof(BalanceQueryResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<BalanceQueryResult>> Balance([FromQuery]BalanceQuery query) => await Mediator.Send(query);

        [HttpGet, ProducesResponseType(typeof(TransactionsQueryResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<TransactionsQueryResult>> Transactions([FromQuery]TransactionsQuery query) => await Mediator.Send(query);

        [HttpPost("/api/[controller]"), ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Create(CreateAccountCommand command) => Convert(await Mediator.Send(command));

        [HttpPost, ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Currency(AddAssetCommand command) => Convert(await Mediator.Send(command));

        [HttpDelete, ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Currency(RemoveAssetCommand command) => Convert(await Mediator.Send(command));

        [HttpPost, ProducesResponseType(typeof(DebitCommandResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<DebitCommandResult>> Debit(DebitCommand command) => await Mediator.Send(command);

        [HttpPost, ProducesResponseType(typeof(CreditCommandResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<CreditCommandResult>> Credit(CreditCommand command) => await Mediator.Send(command);
    }
}
