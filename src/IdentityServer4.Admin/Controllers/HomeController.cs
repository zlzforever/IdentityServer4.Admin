using System;
using System.Threading.Tasks;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.ViewModels.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers
{
    public class HomeController : ControllerBase
    {
        private IDbContext _dbContext;

        public HomeController(IDbContext dbContext, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _dbContext = dbContext;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst("sub").Value;
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

            return Redirect($"user/{userId}/profile");
        }
    }
}