using System.Threading.Tasks;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.Admin.Controllers
{
    public partial class UserController
    {
        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpGet("create")]
        public Task<IActionResult> CreateAsync(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return Task.FromResult<IActionResult>(View("Create", new CreateUserViewModel()));
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync(string returnUrl, CreateUserViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", dto);
            }

            if (dto.UserName == AdminConsts.AdminName)
            {
                ModelState.AddModelError("UserName", $"User name {AdminConsts.AdminName} is not allowed");
                return View("Create", dto);
            }

            var user = _mapper.Map<User>(dto);
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Index");
                }

                return Redirect(returnUrl);
            }

            AddErrors(result);

            return View("Create", dto);
        }
    }
}