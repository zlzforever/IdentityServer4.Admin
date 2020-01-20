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
        [HttpPost("{userId}")]
        public async Task<IActionResult> UpdateAsync(Guid userId, string returnUrl, ViewUserViewModel dto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (user.UserName == AdminConsts.AdminName && dto.UserName != AdminConsts.AdminName)
                {
                    ModelState.AddModelError("UserName", "Admin is not allowed to change user name");
                    return View("View", dto);
                }

                if (user.UserName != AdminConsts.AdminName && dto.UserName == AdminConsts.AdminName)
                {
                    ModelState.AddModelError("UserName", $"User name {AdminConsts.AdminName} is not allowed");
                    return View("View", dto);
                }

                _mapper.Map(dto, user);

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    AddErrors(result);
                    return View("View", dto);
                }

                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Index");
                }

                return Redirect(returnUrl);
            }

            return View("View", dto);
        }
    }
}