using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.ViewModels.Client;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers
{
    public partial class ClientController
    {
        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, string returnUrl, ClientViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                return View("View", dto);
            }

            var client = await _dbContext.Clients
                .Include(x => x.AllowedGrantTypes)
                .Include(x => x.RedirectUris)
                .Include(x => x.PostLogoutRedirectUris)
                .Include(x => x.AllowedScopes)
                .Include(x => x.ClientSecrets)
                .Include(x => x.Claims)
                .Include(x => x.IdentityProviderRestrictions)
                .Include(x => x.AllowedCorsOrigins)
                .Include(x => x.Properties).FirstOrDefaultAsync(x => x.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            var newClient = dto.ToClient();

            var context = (DbContext) _dbContext;
            var transaction = context.Database.BeginTransaction();
            try
            {
                if (client.AllowedGrantTypes != null && client.AllowedGrantTypes.Any())
                {
                    _dbContext.ClientGrantTypes.RemoveRange(client.AllowedGrantTypes);
                }

                if (client.RedirectUris != null && client.RedirectUris.Any())
                {
                    _dbContext.ClientRedirectUris.RemoveRange(client.RedirectUris);
                }

                if (client.PostLogoutRedirectUris != null && client.PostLogoutRedirectUris.Any())
                {
                    _dbContext.ClientPostLogoutRedirectUris.RemoveRange(client.PostLogoutRedirectUris);
                }

                if (client.AllowedScopes != null && client.AllowedScopes.Any())
                {
                    _dbContext.ClientScopes.RemoveRange(client.AllowedScopes);
                }

                if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                {
                    _dbContext.ClientIdPRestrictions.RemoveRange(client.IdentityProviderRestrictions);
                }

                if (client.AllowedCorsOrigins != null && client.AllowedCorsOrigins.Any())
                {
                    _dbContext.ClientCorsOrigins.RemoveRange(client.AllowedCorsOrigins);
                }


                client.AbsoluteRefreshTokenLifetime = newClient.AbsoluteRefreshTokenLifetime;
                client.AccessTokenLifetime = newClient.AccessTokenLifetime;
                client.AccessTokenType = newClient.AccessTokenType;
                client.AllowAccessTokensViaBrowser = newClient.AllowAccessTokensViaBrowser;

                client.AllowOfflineAccess = newClient.AllowOfflineAccess;
                client.AllowPlainTextPkce = newClient.AllowPlainTextPkce;
                client.AllowRememberConsent = newClient.AllowRememberConsent;
                client.AlwaysIncludeUserClaimsInIdToken = newClient.AlwaysIncludeUserClaimsInIdToken;
                client.AlwaysSendClientClaims = newClient.AlwaysSendClientClaims;
                client.AuthorizationCodeLifetime = newClient.AuthorizationCodeLifetime;
                client.BackChannelLogoutSessionRequired = newClient.BackChannelLogoutSessionRequired;
                client.BackChannelLogoutUri = newClient.BackChannelLogoutUri;
                client.ClientClaimsPrefix = newClient.ClientClaimsPrefix;
                client.ClientId = newClient.ClientId;
                client.ClientName = newClient.ClientName;

                if (client.ClientSecrets == null)
                {
                    client.ClientSecrets = new List<ClientSecret>();
                }

                var secrets = dto.ClientSecrets?.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).ToList();
                if (secrets != null)
                {
                    var excludeSecrets = new List<string>();
                    foreach (var clientSecret in client.ClientSecrets)
                    {
                        if (secrets.All(x => x != clientSecret.Value))
                        {
                            _dbContext.ClientSecrets.Remove(clientSecret);
                        }
                        else
                        {
                            excludeSecrets.Add(clientSecret.Value);
                        }
                    }

                    excludeSecrets.ForEach(x => secrets.Remove(x));

                    foreach (var secret in secrets)
                    {
                        var hash = secret.Sha256();
                        // 添加新的密码
                        if (client.ClientSecrets.All(x => x.Value != hash))
                        {
                            client.ClientSecrets.Add(
                                new ClientSecret
                                {
                                    Client = client,
                                    Created = DateTime.Now,
                                    Value = hash
                                });
                        }
                    }
                }

                client.ClientUri = newClient.ClientUri;
                client.ConsentLifetime = newClient.ConsentLifetime;
                client.Description = newClient.Description;
                client.DeviceCodeLifetime = newClient.DeviceCodeLifetime;
                client.Enabled = newClient.Enabled;
                client.EnableLocalLogin = newClient.EnableLocalLogin;
                client.FrontChannelLogoutSessionRequired = newClient.FrontChannelLogoutSessionRequired;
                client.FrontChannelLogoutUri = newClient.FrontChannelLogoutUri;

                client.IdentityTokenLifetime = newClient.IdentityTokenLifetime;
                client.IncludeJwtId = newClient.IncludeJwtId;
                client.LogoUri = newClient.LogoUri;
                client.PairWiseSubjectSalt = newClient.PairWiseSubjectSalt;

                // Properties
                client.ProtocolType = newClient.ProtocolType;

                client.RefreshTokenExpiration = newClient.RefreshTokenExpiration;
                client.RefreshTokenUsage = newClient.RefreshTokenUsage;
                client.RequireClientSecret = newClient.RequireClientSecret;
                client.RequireConsent = newClient.RequireConsent;
                client.RequirePkce = newClient.RequirePkce;
                client.SlidingRefreshTokenLifetime = newClient.SlidingRefreshTokenLifetime;
                client.UpdateAccessTokenClaimsOnRefresh = newClient.UpdateAccessTokenClaimsOnRefresh;
                client.UserCodeType = newClient.UserCodeType;
                client.UserSsoLifetime = newClient.UserSsoLifetime;
                client.AllowedGrantTypes = newClient.AllowedGrantTypes;
                client.RedirectUris = newClient.RedirectUris;
                client.PostLogoutRedirectUris = newClient.PostLogoutRedirectUris;
                client.AllowedScopes = newClient.AllowedScopes;
                client.IdentityProviderRestrictions = newClient.IdentityProviderRestrictions;
                client.AllowedCorsOrigins = newClient.AllowedCorsOrigins;

                _dbContext.Clients.Update(client);
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
                Logger.LogError($"Update client failed: {e}");
                try
                {
                    transaction.Rollback();
                }
                catch (Exception te)
                {
                    Logger.LogError($"Rollback update client failed: {te}");
                }

                ModelState.AddModelError(string.Empty, "Update client failed");
                return View("View", dto);
            }
        }
    }
}