using System.Threading.Tasks;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers
{
    public partial class AccountController
    {
        [HttpGet("register")]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                if (!string.Equals(HttpContext.Session.GetString(AdminConsts.VerificationCode)?.ToLower(),
                    model.VerificationCode?.ToLower()))
                {
                    ModelState.AddModelError(AdminConsts.VerificationCode, "验证码不正确");
                }
                else if (model.AgreeTerms != "true")
                {
                    ModelState.AddModelError("AgreeTerms", "请接受网站条款");
                }
                else
                {
                    var user = new User {UserName = model.UserName.Trim()};
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                        // Send an email with this link
                        //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                        //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                        //    "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        Logger.LogInformation(3, "User created a new account with password.");
                        return RedirectToLocal(returnUrl);
                    }

                    AddErrors(result);
                }
            }

            ViewData["ReturnUrl"] = returnUrl;
            // If we got this far, something failed, redisplay form
            return View(model);
        }
    }
}