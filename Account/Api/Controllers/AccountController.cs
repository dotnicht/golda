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
        public async Task<IActionResult> Currency(AddCurrencyCommand command) => Convert(await Mediator.Send(command));

        [HttpDelete, ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Currency(RemoveCurrencyCommand command) => Convert(await Mediator.Send(command));

        [HttpPost, ProducesResponseType(typeof(DebitAccountCommandResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<DebitAccountCommandResult>> Debit(DebitAccountCommand command) => await Mediator.Send(command);

        [HttpPost, ProducesResponseType(typeof(CreditAccountCommandResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<CreditAccountCommandResult>> Credit(CreditAccountCommand command) => await Mediator.Send(command);
    }
}
