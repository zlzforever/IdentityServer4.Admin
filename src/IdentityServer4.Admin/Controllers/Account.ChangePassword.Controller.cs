using System.Threading.Tasks;
using IdentityServer4.Admin.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.Admin.Controllers
{
    public partial class AccountController
    {
        [HttpGet("change-password")]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                return View("changepassword", dto);
            }

            dto.NewPassword = dto.NewPassword.Trim();
            dto.OldPassword = dto.OldPassword.Trim();

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == UserId);
            if (user == null)
            {
                return BadRequest(new {Code = 400, Msg = "User not exists"});
            }

            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                AddErrors(result);
                return View("changepassword", dto);
            }
            else
            {
                return View("changepassword", new ChangePasswordViewModel());
            }
        }
    }
}