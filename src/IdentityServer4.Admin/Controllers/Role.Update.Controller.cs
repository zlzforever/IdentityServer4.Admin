using System;
using System.Threading.Tasks;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.ViewModels.Role;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.Admin.Controllers
{
    public partial class RoleController
    {
        [HttpPost("{roleId}")]
        public async Task<IActionResult> UpdateAsync(Guid roleId, string returnUrl, RoleViewModel dto)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(p => p.Id == roleId);
            if (role == null)
            {
                return NotFound();
            }

            if (role.Name == AdminConsts.AdminName && role.Name != AdminConsts.AdminName)
            {
                ModelState.AddModelError("Name", "Admin is not allowed to change name");
                return View("View", dto);
            }

            if (role.Name != AdminConsts.AdminName && dto.Name == AdminConsts.AdminName)
            {
                ModelState.AddModelError("Name", $"Role name must not be {AdminConsts.AdminName}");
                return View("View", dto);
            }

            string normalizedName = _roleManager.NormalizeKey(role.Name);
            if (await _roleManager.Roles.AnyAsync(u =>
                u.Id != roleId && u.NormalizedName == normalizedName))
            {
                ModelState.AddModelError("Name", "Name already exists");
                return View("View", dto);
            }

            role.Name = dto.Name;
            role.Description = dto.Description;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Index");
                }

                return Redirect(returnUrl);
            }

            AddErrors(result);
            return View("View", dto);
        }
    }
}