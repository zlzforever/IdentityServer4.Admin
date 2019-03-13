using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.ViewModels.IdentityResource;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers
{
    public partial class IdentityResourceController
    {
        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, string returnUrl, IdentityResourceViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                return View("View", dto);
            }

            var identityResource = await _dbContext.IdentityResources.FirstOrDefaultAsync(x => x.Id == id);
            if (identityResource == null)
            {
                return NotFound();
            }

            dto.Name = dto.Name.Trim();
            var nameExits = await _dbContext.IdentityResources.AnyAsync(x => x.Id != id && x.Name == dto.Name);
            if (nameExits)
            {
                ModelState.AddModelError(string.Empty, $"Resource name {dto.Name} already exists");
                return View("View", dto);
            }

            identityResource.Name = dto.Name;
            identityResource.ShowInDiscoveryDocument = dto.ShowInDiscoveryDocument;
            identityResource.Description = dto.Description?.Trim();
            identityResource.DisplayName =
                string.IsNullOrWhiteSpace(dto.DisplayName) ? dto.Name : dto.DisplayName?.Trim();
            identityResource.Emphasize = dto.Emphasize;
            identityResource.Enabled = dto.Enabled;
            identityResource.Required = dto.Required;

            var oldClaims = await _dbContext.IdentityClaims.Where(x => x.IdentityResourceId == identityResource.Id)
                .ToListAsync();

            var context = (DbContext) _dbContext;
            var transaction = context.Database.BeginTransaction();
            try
            {
                _dbContext.IdentityClaims.RemoveRange(oldClaims);

                var newClaims = dto.UserClaims?.Split(" ", StringSplitOptions.RemoveEmptyEntries);

                var identityClaims = new List<IdentityClaim>();
                if (newClaims != null)
                {
                    var list = newClaims.ToList();
                    list.Sort();
                    foreach (var identityClaim in list)
                    {
                        identityClaims.Add(new IdentityClaim
                        {
                            Type = identityClaim,
                            IdentityResource = identityResource
                        });
                    }
                }

                identityResource.UserClaims = identityClaims;
                _dbContext.IdentityResources.Update(identityResource);
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
                Logger.LogError($"Update identity resource failed: {e}");
                try
                {
                    transaction.Rollback();
                }
                catch (Exception te)
                {
                    Logger.LogError($"Rollback update identity resource failed: {te}");
                }

                ModelState.AddModelError(string.Empty, "Update identity resource failed");
                return View("View", dto);
            }
        }
    }
}