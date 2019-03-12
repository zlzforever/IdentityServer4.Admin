using System.Threading.Tasks;
using IdentityServer4.Admin.ViewModels;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers
{
    public class ErrorController : ControllerBase
    {
        private readonly IIdentityServerInteractionService _interaction;

        public ErrorController(IIdentityServerInteractionService interaction, ILoggerFactory loggerFactory) : base(
            loggerFactory)
        {
            _interaction = interaction;
        }

        /// <summary>
        /// Shows the error page
        /// </summary>
        public async Task<IActionResult> Index(string errorId)
        {
            var vm = new ErrorViewModel();

            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;
            }

            return View("Error", vm);
        }
    }
}