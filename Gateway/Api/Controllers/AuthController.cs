using Binebase.Exchange.Common.Api.Controllers;
using Binebase.Exchange.Gateway.Application.Commands;
using Binebase.Exchange.Gateway.Application.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Api.Controllers
{
    public class AuthController : ApiController
    {
        [HttpPost, ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> SignUp(SignUpCommand command) 
            => Convert(await Mediator.Send(command));

        [HttpPost, ProducesResponseType(typeof(SignInCommandResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<SignInCommandResult>> SignIn(SignInCommand command) 
            => await Mediator.Send(command);

        [HttpPost, ProducesResponseType(typeof(SignInCommandResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<SignInCommandResult>> SignInMultiFactor(SignInMultiFactorCommand command)
            => await Mediator.Send(command);

        [HttpPost, ProducesResponseType(typeof(SignInCommandResult), StatusCodes.Status200OK)]
        public async Task<SignInCommandResult> Confirm(ConfirmCommand command) 
            => await Mediator.Send(command);

        [HttpPost, ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Resend(ResendCommand command) 
            => Convert(await Mediator.Send(command));

        [HttpPost, ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RequestResetPassword(RequestResetPasswordCommand command) 
            => Convert(await Mediator.Send(command));

        [HttpPost, ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command) 
            => Convert(await Mediator.Send(command));

        [HttpGet, Authorize, ProducesResponseType(typeof(MultiFactorStatusQueryResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<MultiFactorStatusQueryResult>> MultiFactorStatus([FromQuery]MultiFactorStatusQuery query) 
            => await Mediator.Send(query);

        [HttpPost, Authorize, ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MultiFactor(EnableMultiFactorCommand command) 
            => Convert(await Mediator.Send(command));

        [HttpDelete, Authorize, ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MultiFactor(DisableMultiFactorCommand command) 
            => Convert(await Mediator.Send(command));
    }
}