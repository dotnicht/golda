using Binebase.Exchange.Common.Api.Controllers;
using Binebase.Exchange.CryptoService.Application.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Api.Controllers
{
    public class CryptoController : ApiController
    {
        [HttpPost, ProducesResponseType(typeof(GenerateAddressCommandResult), StatusCodes.Status200OK)]
        public async Task<ActionResult<GenerateAddressCommandResult>> Address(GenerateAddressCommand command)
            => await Mediator.Send(command);
    }
}
