using System;
using System.Collections.Generic;
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
        [HttpGet("create")]
        public Task<IActionResult> CreateAsync(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return Task.FromResult((IActionResult) View("Create", new CreateApiResourceViewModel()));
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync(string returnUrl, CreateApiResourceViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", dto);
            }

            dto.Name = dto.Name.Trim();
            if (await _dbContext.ApiResources.AnyAsync(x => x.Name == dto.Name))
            {
                ModelState.AddModelError("Name", "Name already exists");
                return View("Create", dto);
            }

            var identityResource = new ApiResource
            {
                Name = dto.Name,
                Description = dto.Description?.Trim(),
                DisplayName = string.IsNullOrWhiteSpace(dto.DisplayName) ? dto.Name : dto.DisplayName?.Trim(),
                Enabled = dto.Enabled
            };
            var context = (DbContext) _dbContext;
            var transaction = context.Database.BeginTransaction();
            try
            {
                var claims = dto.UserClaims?.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var identityClaims = new List<ApiResourceClaim>();
                if (claims != null)
                {
                    foreach (var identityClaim in claims)
                    {
                        identityClaims.Add(new ApiResourceClaim
                        {
                            Type = identityClaim,
                            ApiResource = identityResource
                        });
                    }
                }

                identityResource.UserClaims = identityClaims;
                await _dbContext.ApiResources.AddAsync(identityResource);
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
                Logger.LogError($"Create api resource failed: {e}");
                try
                {
                    transaction.Rollback();
                }
                catch (Exception te)
                {
                    Logger.LogError($"Rollback api resource failed: {te}");
                }

                ModelState.AddModelError(string.Empty, "Create api resource failed");
                return View("Create", dto);
            }
        }
    }
}