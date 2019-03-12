using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers
{
    public class ControllerBase : Controller
    {
        protected ILogger Logger { get; }

        protected ControllerBase(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<ControllerBase>();
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ViewData["IsAdmin"] = IsAdmin();

            return base.OnActionExecutionAsync(context, next);
        }

        protected bool IsAdmin()
        {
            return User.IsInRole(AdminConsts.AdminName);
        }

        protected Guid UserId => HttpContext.User.Identity.GetUserId<Guid>();

        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        protected IActionResult IdentityResult(IdentityResult identityResult)
        {
            if (identityResult.Succeeded)
            {
                return Ok(new {Code = 200, Msg = "success"});
            }

            return BadRequest(new {Code = 400, Msg = identityResult.Errors.First().Description});
        }
    }
}