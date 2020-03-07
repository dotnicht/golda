using Binebase.Exchange.Common.Api.Controllers;
using Binebase.Exchange.Gateway.Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Api.Controllers
{
    [Authorize]
    public class AccountController : ApiController
    {
        [HttpGet]
        public async Task<ActionResult<BalanceQueryResult>> Balance([FromQuery]BalanceQuery query) 
            => await Mediator.Send(query);

        [HttpGet]
        public async Task<ActionResult<PortfolioQueryResult>> Portfolio([FromQuery]PortfolioQuery query) 
            => await Mediator.Send(query);

        [HttpGet]
        public async Task<ActionResult<TransactionsQueryResult>> Transactions([FromQuery]TransactionsQuery query) 
            => await Mediator.Send(query);
    }
}
