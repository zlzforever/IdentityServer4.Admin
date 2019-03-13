using System;
using System.Collections.Generic;
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
        [HttpGet("create")]
        public Task<IActionResult> CreateAsync(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return Task.FromResult((IActionResult) View("Create", new IdentityResourceViewModel()));
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync(string returnUrl, IdentityResourceViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", dto);
            }

            var identityResource = new IdentityResource
            {
                Name = dto.Name.Trim(),
                ShowInDiscoveryDocument = dto.ShowInDiscoveryDocument,
                Description = dto.Description?.Trim(),
                DisplayName = string.IsNullOrWhiteSpace(dto.DisplayName) ? dto.Name.Trim() : dto.DisplayName?.Trim(),
                Emphasize = dto.Emphasize,
                Enabled = dto.Enabled,
                Required = dto.Required
            };
            var context = (DbContext) _dbContext;
            var transaction = context.Database.BeginTransaction();
            try
            {
                var claims = dto.UserClaims?.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var identityClaims = new List<IdentityClaim>();
                if (claims != null)
                {
                    foreach (var identityClaim in claims)
                    {
                        identityClaims.Add(new IdentityClaim
                        {
                            Type = identityClaim,
                            IdentityResource = identityResource
                        });
                    }
                }

                identityResource.UserClaims = identityClaims;
                await _dbContext.IdentityResources.AddAsync(identityResource);
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
                Logger.LogError($"Create identity resource failed: {e}");
                try
                {
                    transaction.Rollback();
                }
                catch (Exception te)
                {
                    Logger.LogError($"Rollback create identity resource failed: {te}");
                }

                ModelState.AddModelError(string.Empty, "Create identity resource failed");
                return View("Create", dto);
            }
        }
    }
}