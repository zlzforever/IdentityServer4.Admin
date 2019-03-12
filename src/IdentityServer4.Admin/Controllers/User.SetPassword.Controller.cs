using System;
using System.Threading.Tasks;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.Admin.Controllers
{
    public partial class UserController
    {
        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpGet("{userId}/set-password")]
        public IActionResult SetPassword(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new SetPasswordViewModel());
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpPost("{userId}/set-password")]
        public async Task<IActionResult> SetPassword(Guid userId, string returnUrl, SetPasswordViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                return View("SetPassword", dto);
            }

            dto.NewPassword = dto.NewPassword.Trim();

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            if (user.UserName == AdminConsts.AdminName)
            {
                return StatusCode(500);
            }

            var identityResult = await _userManager.RemovePasswordAsync(user);
            if (!identityResult.Succeeded)
            {
                AddErrors(identityResult);
                return View("SetPassword", dto);
            }

            identityResult = await _userManager.AddPasswordAsync(user, dto.NewPassword.Trim());
            if (!identityResult.Succeeded)
            {
                AddErrors(identityResult);
                return View("SetPassword", dto);
            }

            if (string.IsNullOrEmpty(returnUrl))
            {
                return View(new SetPasswordViewModel());
            }

            return Redirect(returnUrl);
        }
    }
}