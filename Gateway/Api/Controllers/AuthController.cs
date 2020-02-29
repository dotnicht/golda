using Binebase.Exchange.Gateway.Application.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Api.Controllers
{
    public class AuthController : ApiController
    {
        [HttpPost, ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> SignUp(SignUpCommand command) => Convert(await Mediator.Send(command));
        [HttpPost, ProducesResponseType(typeof(SignInCommandResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<SignInCommandResult>> SignIn(SignInCommand command) => await Mediator.Send(command);
        [HttpPost, ProducesResponseType(typeof(ConfirmCommandResult), StatusCodes.Status200OK)]
        public async Task<ConfirmCommandResult> Confirm(ConfirmCommand command) => await Mediator.Send(command);
        [HttpPost, ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Resend(ResendCommand command) => Convert(await Mediator.Send(command));
        [HttpPost, ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> SendPasswordReset(SendPasswordResetCommand command) => Convert(await Mediator.Send(command));
        [HttpPost, ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command) => Convert(await Mediator.Send(command));
    }
}