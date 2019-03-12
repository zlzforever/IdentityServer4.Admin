using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.ViewModels.Client;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.Admin.Controllers
{
    public partial class ClientController
    {
        [HttpGet("create")]
        public Task<IActionResult> CreateAsync(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return Task.FromResult<IActionResult>(View("Create", new CreateClientViewModel()));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync(string returnUrl, CreateClientViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", dto);
            }

            if (await _dbContext.Clients.AnyAsync(u => u.ClientId == dto.ClientId))
            {
                ModelState.AddModelError("ClientId", "Client already exits");
                return View("Create", dto);
            }

            var redirectUris = dto.RedirectUris.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries)
                .Where(cors => !string.IsNullOrWhiteSpace(cors) && cors.IsUrl()).ToList();
            if (redirectUris.Count == 0)
            {
                ModelState.AddModelError("RedirectUris", "At least one valid redirect url");
                return View("Create", dto);
            }

            var allowedCorsOrigins = dto.AllowedCorsOrigins.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries)
                .Where(cors => !string.IsNullOrWhiteSpace(cors) && cors.IsUrl()).ToList();
            if (allowedCorsOrigins.Count == 0)
            {
                ModelState.AddModelError("AllowedCorsOrigins", "At least one valid cors origin");
                return View("Create", dto);
            }


            var client = new Client
            {
                AllowedGrantTypes = dto.GetAllowedGrantTypes(),
                Description = dto.Description,
                AllowedScopes = dto.AllowedScopes.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)
                    .Where(cors => !string.IsNullOrWhiteSpace(cors)).ToList(),
                ClientId = dto.ClientId,
                ClientName = dto.ClientName,
                ClientUri = dto.ClientUri,
                ConsentLifetime = dto.ConsentLifetime,
                LogoUri = dto.LogoUri,
                ProtocolType = dto.ProtocolType,
                RedirectUris = redirectUris,
                RequireConsent = dto.RequireConsent,
                RequirePkce = dto.RequirePkce,
                AccessTokenLifetime = dto.AccessTokenLifetime,
                AccessTokenType = dto.AccessTokenType,
                AllowedCorsOrigins = allowedCorsOrigins,
                AllowOfflineAccess = dto.AllowOfflineAccess,
                AllowRememberConsent = dto.AllowRememberConsent,
                AuthorizationCodeLifetime = dto.AuthorizationCodeLifetime,
                ClientClaimsPrefix = dto.ClientClaimsPrefix,
                DeviceCodeLifetime = dto.DeviceCodeLifetime,
                EnableLocalLogin = dto.EnableLocalLogin,
                IdentityProviderRestrictions =
                    dto.IdentityProviderRestrictions?.Split(" ", StringSplitOptions.RemoveEmptyEntries),
                IdentityTokenLifetime = dto.IdentityTokenLifetime,
                IncludeJwtId = dto.IncludeJwtId,
                RefreshTokenExpiration = dto.RefreshTokenExpiration,
                RefreshTokenUsage = dto.RefreshTokenUsage,
                RequireClientSecret = dto.RequireClientSecret,
                UserCodeType = dto.UserCodeType,
                UserSsoLifetime = dto.UserSsoLifetime,
                AbsoluteRefreshTokenLifetime = dto.AbsoluteRefreshTokenLifetime,
                AllowPlainTextPkce = dto.AllowPlainTextPkce,
                AlwaysSendClientClaims = dto.AlwaysSendClientClaims,
                BackChannelLogoutUri = dto.BackChannelLogoutUri,
                FrontChannelLogoutUri = dto.FrontChannelLogoutUri,
                PairWiseSubjectSalt = dto.PairWiseSubjectSalt,
                PostLogoutRedirectUris = dto.PostLogoutRedirectUris?
                    .Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries)
                    .Where(cors => !string.IsNullOrWhiteSpace(cors) && cors.IsUrl()).ToList(),
                SlidingRefreshTokenLifetime = dto.SlidingRefreshTokenLifetime,
                AllowAccessTokensViaBrowser = dto.AllowAccessTokensViaBrowser,
                BackChannelLogoutSessionRequired = dto.BackChannelLogoutSessionRequired,
                FrontChannelLogoutSessionRequired = dto.FrontChannelLogoutSessionRequired,
                UpdateAccessTokenClaimsOnRefresh = dto.UpdateAccessTokenClaimsOnRefresh,
                AlwaysIncludeUserClaimsInIdToken = dto.AlwaysIncludeUserClaimsInIdToken
            };

            await _dbContext.Clients.AddAsync(client.ToEntity());
            await _dbContext.SaveChangesAsync();
            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Index");
            }

            return Redirect(returnUrl);
        }
    }
}