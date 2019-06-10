using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.ViewModels.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers
{
    public partial class AccountController
    {
        [HttpPost("head-icon")]
        public async Task<IActionResult> HeadIcon()
        {
            using (var reader = new StreamReader(HttpContext.Request.Body))
            {
                var base64 = await reader.ReadToEndAsync();
                var path = $"{_options.StorageRoot}/head-icon/{UserId}.png";
                base64 = base64.Replace("data:image/png;base64,", "").Replace("data:image/jgp;base64,", "")
                    .Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "");
                byte[] bytes = Convert.FromBase64String(base64);
                await System.IO.File.WriteAllBytesAsync(path, bytes);

                var user = await _userManager.FindByIdAsync(UserId.ToString());
                user.Icon = $"head-icon/{UserId}.png";
                await _userManager.UpdateAsync(user);
                return StatusCode(200, user.Icon);
            }
        }

        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == UserId);
            return user == null ? (IActionResult) NotFound() : View(await BuildProfileViewModelAsync(user));
        }

        [HttpPost("profile")]
        public async Task<IActionResult> Profile(UpdateProfileViewModel dto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == UserId);
            if (user == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Mapper.Map(dto, user);
                await _userManager.UpdateAsync(user);
            }

            var viewModel = await BuildProfileViewModelAsync(user);
            Mapper.Map(dto, viewModel);
            return RedirectToAction("Profile");
        }

        private async Task<ProfileViewModel> BuildProfileViewModelAsync(User user)
        {
            string icon = Path.Combine("head-icon", "default.png");

            if (!string.IsNullOrWhiteSpace(user.Icon))
            {
                try
                {
                    var realPath = Path.Combine(_options.StorageRoot, user.Icon);
                    if (System.IO.File.Exists(realPath))
                    {
                        icon = user.Icon;
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError($"Calculate head icon path failed: {e}");
                }
            }

            var viewModel = new ProfileViewModel
            {
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                UserName = user.UserName,
                Sex = user.Sex,
                Title = user.Title,
                GivenName = user.GivenName,
                FamilyName = user.FamilyName,
                OfficePhone = user.OfficePhone,
                Icon = icon,
                NickName = user.NickName,
                WebSite = user.WebSite,
                Slogan = user.Slogan,
                Location = user.Location,
                Roles = string.Join("; ", await _userManager.GetRolesAsync(user))
            };
            return viewModel;
        }
    }
}