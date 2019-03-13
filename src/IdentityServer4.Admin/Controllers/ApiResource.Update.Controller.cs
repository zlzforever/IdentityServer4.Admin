using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.ViewModels.ApiResource;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers
{
    public partial class ApiResourceController
    {
        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, string returnUrl, ApiResourceViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                return View("View", dto);
            }

            var resource = await _dbContext.ApiResources.FirstOrDefaultAsync(x => x.Id == id);
            if (resource == null)
            {
                return NotFound();
            }

            dto.Name = dto.Name.Trim();
            var nameExits = await _dbContext.ApiResources.AnyAsync(x => x.Id != id && x.Name == dto.Name);
            if (nameExits)
            {
                ModelState.AddModelError(string.Empty, $"Resource name {dto.Name} already exists");
                return View("View", dto);
            }

            resource.Name = dto.Name;

            resource.Description = dto.Description?.Trim();
            resource.DisplayName =
                string.IsNullOrWhiteSpace(dto.DisplayName) ? dto.Name : dto.DisplayName?.Trim();
            resource.Enabled = dto.Enabled;

            var oldClaims = await _dbContext.ApiResourceClaims.Where(x => x.ApiResourceId == resource.Id)
                .ToListAsync();

            var context = (DbContext) _dbContext;
            var transaction = context.Database.BeginTransaction();
            try
            {
                _dbContext.ApiResourceClaims.RemoveRange(oldClaims);

                var newClaims = dto.UserClaims?.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                var claims = new List<ApiResourceClaim>();
                if (newClaims != null)
                {
                    var list = newClaims.ToList();
                    list.Sort();
                    foreach (var identityClaim in list)
                    {
                        claims.Add(new ApiResourceClaim
                        {
                            Type = identityClaim,
                            ApiResource = resource
                        });
                    }
                }

                resource.UserClaims = claims;
                _dbContext.ApiResources.Update(resource);
                await _dbContext.SaveChangesAsync();

                transaction.Commit();
                if (string.IsNullOrEmpty(returnUrl))
                {
                    return RedirectToAction("Index");
                }

                return Redirect(returnUrl);
            }
            catch (Exception e)
            {
                Logger.LogError($"Update api resource failed: {e}");
                try
                {
                    transaction.Rollback();
                }
                catch (Exception te)
                {
                    Logger.LogError($"Rollback update api resource failed: {te}");
                }

                ModelState.AddModelError(string.Empty, "Update api resource failed");
                return View("View", dto);
            }
        }
    }
}