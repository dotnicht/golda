using Binebase.Exchange.Gateway.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Admin.Pages
{
    public class TransferModel : PageModel
    {
        private readonly ICryptoService _cryptoService;
        private readonly ILogger _logger;

        public TransferModel(ILogger<TransferModel> logger, ICryptoService cryptoService)
        {
            _logger = logger;
            _cryptoService = cryptoService;
        }

        public void OnGet()
        {          
        }

        public async Task OnPostRunTransfer() 
        {
            _logger.LogDebug("Run transfer operation");
             await _cryptoService.Transfer();
        }
    }
}