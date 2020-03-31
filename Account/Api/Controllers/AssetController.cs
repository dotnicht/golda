using Binebase.Exchange.AccountService.Contracts.Commands;
using Binebase.Exchange.Common.Api.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Binebase.Exchange.AccountService.Api.Controllers
{
    public class AssetController : ApiController
    {
        [HttpPost, ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Lock(LockAssetCommand command) => Convert(await Mediator.Send(command));

        [HttpPost, ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Unlock(UnlockAssetCommand command) => Convert(await Mediator.Send(command));

        [HttpPost, ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Currency(AddAssetCommand command) => Convert(await Mediator.Send(command));

        [HttpDelete, ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Currency(RemoveAssetCommand command) => Convert(await Mediator.Send(command));
    }
}
