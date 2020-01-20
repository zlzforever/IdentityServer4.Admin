using System;
using System.Threading.Tasks;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.ViewModels;
using IdentityServer4.Admin.ViewModels.Home;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly IDbContext _dbContext;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IHostEnvironment _environment;

        public HomeController(IIdentityServerInteractionService interaction, IDbContext dbContext,
            IHostEnvironment environment, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _dbContext = dbContext;
            _interaction = interaction;
            _environment = environment;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (HttpContext.User.IsInRole(AdminConsts.AdminName))
            {
                var viewModel = new DashboardViewModel
                {
                    ApiResourceCount = await _dbContext.ApiResources.CountAsync(),
                    ClientCount = await _dbContext.Clients.CountAsync(),
                    LockedUserCount = await _dbContext.Users.CountAsync(u => u.LockoutEnd < DateTime.Now),
                    UserCount = await _dbContext.Users.CountAsync()
                };

                return View(viewModel);
            }

            return Redirect("account/profile");
        }

        /// <summary>
        /// Shows the error page
        /// </summary>
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;

                if (!_environment.IsDevelopment())
                {
                    // only show in development
                    message.ErrorDescription = null;
                }
            }

            return View("Error", vm);
        }
    }
}