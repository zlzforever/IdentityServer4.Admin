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
        [HttpGet("{id}/scope")]
        public async Task<IActionResult> ScopeAsync(int id)
        {
            var resource = await _dbContext.ApiResources.FirstOrDefaultAsync(u => u.Id == id);
            if (resource == null)
            {
                return NotFound();
            }

            var scopes = await _dbContext.ApiScopes.Where(x => x.ApiResourceId == id).ToListAsync();
            var ids = scopes.Select(x => x.Id);
            var claims = await _dbContext.ApiScopeClaims.Where(x => ids.Contains(x.ApiScopeId))
                .GroupBy(x => x.ApiScopeId).ToDictionaryAsync(g => g.Key);

            var viewModel = new List<ListApiResourceScopeViewModel>();
            foreach (var scope in scopes)
            {
                viewModel.Add(new ListApiResourceScopeViewModel
                {
                    Id = scope.Id,
                    Name = scope.Name,
                    Description = scope.Description,
                    DisplayName = scope.DisplayName,
                    ShowInDiscoveryDocument = scope.ShowInDiscoveryDocument,
                    Emphasize = scope.Emphasize,
                    Required = scope.Required,
                    UserClaims = claims.ContainsKey(scope.Id)
                        ? string.Join(" ", claims[scope.Id].Select(x => x.Type))
                        : string.Empty
                });
            }

            ViewData["ApiResourceId"] = id;

            return View("Scope", viewModel);
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpGet("{id}/scope/create")]
        public async Task<IActionResult> CreateScopeAsync(int id)
        {
            var resource = await _dbContext.ApiResources.FirstOrDefaultAsync(x => x.Id == id);
            if (resource == null)
            {
                return NotFound();
            }

            ViewData["ApiResourceId"] = id;
            return View("CreateScope", new CreateApiResourceScopeViewModel());
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpPost("{id}/scope/create")]
        public async Task<IActionResult> CreateScopeAsync(int id, CreateApiResourceScopeViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateScope", dto);
            }

            var resource = await _dbContext.ApiResources.FirstOrDefaultAsync(u => u.Id == id);
            if (resource == null)
            {
                return NotFound();
            }

            dto.Name = dto.Name.Trim();
            var scope = new ApiScope
            {
                Name = dto.Name,
                Description = dto.Description?.Trim(),
                DisplayName = string.IsNullOrWhiteSpace(dto.DisplayName) ? dto.Name : dto.DisplayName?.Trim(),
                Required = dto.Required,
                ShowInDiscoveryDocument = dto.ShowInDiscoveryDocument,
                Emphasize = dto.Emphasize,
                ApiResource = resource
            };
            var context = (DbContext) _dbContext;
            var transaction = context.Database.BeginTransaction();
            try
            {
                var claimTypes = dto.UserClaims?.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var claims = new List<ApiScopeClaim>();
                if (claimTypes != null)
                {
                    foreach (var claimType in claimTypes)
                    {
                        claims.Add(new ApiScopeClaim
                        {
                            Type = claimType,
                            ApiScope = scope
                        });
                    }
                }

                scope.UserClaims = claims;
                await _dbContext.ApiScopes.AddAsync(scope);
                await _dbContext.SaveChangesAsync();

                transaction.Commit();
                return Redirect($"/api-resource/{id}/scope");
            }
            catch (Exception e)
            {
                Logger.LogError($"Add scope failed: {e}");
                try
                {
                    transaction.Rollback();
                }
                catch (Exception te)
                {
                    Logger.LogError($"Rollback add scope failed: {te}");
                }

                ModelState.AddModelError(string.Empty, "Add scope failed");
                return View("CreateScope", dto);
            }
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpGet("{resourceId}/scope/{id}")]
        public async Task<IActionResult> ScopeAsync(int resourceId, int id)
        {
            var resource = await _dbContext.ApiResources.FirstOrDefaultAsync(x => x.Id == resourceId);
            if (resource == null)
            {
                return BadRequest();
            }

            var scope = await _dbContext.ApiScopes.FirstOrDefaultAsync(x => x.Id == id);
            if (scope == null)
            {
                return BadRequest();
            }

            if (scope.ApiResourceId != resourceId)
            {
                return BadRequest();
            }

            var viewModel = new ViewApiResourceScopeViewModel
            {
                Name = scope.Name,
                Required = scope.Required,
                Emphasize = scope.Emphasize,
                Description = scope.Description,
                ShowInDiscoveryDocument = scope.ShowInDiscoveryDocument,
                DisplayName = scope.DisplayName
            };
            var claims = await _dbContext.ApiScopeClaims.Where(x => x.ApiScopeId == id).Select(x => x.Type)
                .ToListAsync();
            viewModel.UserClaims = string.Join(" ", claims);
            ViewData["ApiResource"] = resource.Name;
            ViewData["ApiResourceId"] = resourceId;
            ViewData["ScopeId"] = id;
            return View("ViewScope", viewModel);
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpPost("{resourceId}/scope/{id}")]
        public async Task<IActionResult> UpdateScopeAsync(int resourceId, int id, string returnUrl,
            ViewApiResourceScopeViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                return View("ViewScope", dto);
            }

            var resource = await _dbContext.ApiResources.FirstOrDefaultAsync(x => x.Id == resourceId);
            if (resource == null)
            {
                return BadRequest();
            }

            var scope = await _dbContext.ApiScopes.FirstOrDefaultAsync(x => x.Id == id);
            if (scope == null)
            {
                return BadRequest();
            }

            if (scope.ApiResourceId != resourceId)
            {
                return BadRequest();
            }

            dto.Name = dto.Name.Trim();
            scope.Name = dto.Name;
            scope.Description = dto.Description?.Trim();
            scope.DisplayName = string.IsNullOrWhiteSpace(dto.DisplayName) ? dto.Name : dto.DisplayName?.Trim();
            scope.Required = dto.Required;
            scope.ShowInDiscoveryDocument = dto.ShowInDiscoveryDocument;
            scope.Emphasize = dto.Emphasize;
            var oldClaims = await _dbContext.ApiScopeClaims.Where(x => x.ApiScopeId == scope.Id)
                .ToListAsync();
            var context = (DbContext) _dbContext;
            var transaction = context.Database.BeginTransaction();
            try
            {
                _dbContext.ApiScopeClaims.RemoveRange(oldClaims);

                var claimTypes = dto.UserClaims?.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var claims = new List<ApiScopeClaim>();
                if (claimTypes != null)
                {
                    var list = claimTypes.ToList();
                    list.Sort();
                    foreach (var claimType in list)
                    {
                        claims.Add(new ApiScopeClaim
                        {
                            Type = claimType,
                            ApiScope = scope
                        });
                    }
                }

                scope.UserClaims = claims;
                _dbContext.ApiScopes.Update(scope);
                await _dbContext.SaveChangesAsync();
                transaction.Commit();
                return Redirect($"/api-resource/{resourceId}/scope");
            }
            catch (Exception e)
            {
                Logger.LogError($"Update scope failed: {e}");
                try
                {
                    transaction.Rollback();
                }
                catch (Exception te)
                {
                    Logger.LogError($"Rollback update scope failed: {te}");
                }

                ModelState.AddModelError(string.Empty, "Update scope failed");
                return View("ViewScope", dto);
            }
        }
    }
}