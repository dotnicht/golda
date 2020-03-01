using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Binebase.Exchange.Common.Api.Controllers
{
    [ApiController, Route("api/[controller]/[action]")]
    public abstract class ApiController : ControllerBase
    {
        private IMediator _mediator;

        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        protected IActionResult Convert(Unit _) => NoContent();
    }
}
