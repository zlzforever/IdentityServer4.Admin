using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.ViewModels.Client;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using X.PagedList;

namespace IdentityServer4.Admin.Controllers
{
    [Authorize(Roles = AdminConsts.AdminName)]
    [Route("client")]
    public partial class ClientController : ControllerBase
    {
        private readonly IDbContext _dbContext;

        public ClientController(IDbContext dbContext, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index(PagedQuery input)
        {
            var queryResult = await _dbContext.Clients
                .Include(x => x.AllowedGrantTypes)
                .Include(x => x.RedirectUris)
                .Include(x => x.PostLogoutRedirectUris)
                .Include(x => x.AllowedScopes)
                .Include(x => x.ClientSecrets)
                .Include(x => x.Claims)
                .Include(x => x.IdentityProviderRestrictions)
                .Include(x => x.AllowedCorsOrigins)
                .Include(x => x.Properties)
                .AsNoTracking().Select(x => new ListClientItemViewModel
                {
                    Id = x.Id,
                    ClientId = x.ClientId,
                    ClientName = x.ClientName,
                    AllowedScopes = string.Join(" ", x.AllowedScopes.Select(s => s.Scope)),
                    AllowedGrantTypes = string.Join(" ", x.AllowedGrantTypes.Select(t => t.GrantType))
                })
                .ToPagedListAsync(input.GetPage(), input.GetSize());
            return View(queryResult);
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpGet("{id}")]
        public async Task<IActionResult> ViewAsync(int id, string returnUrl)
        {
            var client = await _dbContext.Clients
                .Include(x => x.AllowedGrantTypes)
                .Include(x => x.RedirectUris)
                .Include(x => x.PostLogoutRedirectUris)
                .Include(x => x.AllowedScopes)
                .Include(x => x.ClientSecrets)
                .Include(x => x.Claims)
                .Include(x => x.IdentityProviderRestrictions)
                .Include(x => x.AllowedCorsOrigins)
                .Include(x => x.Properties)
                .AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            // 47个参数
            var viewModel = new ClientViewModel();
            viewModel.AbsoluteRefreshTokenLifetime = client.AbsoluteRefreshTokenLifetime;
            viewModel.AccessTokenLifetime = client.AccessTokenLifetime;
            viewModel.AccessTokenType = (AccessTokenType) client.AccessTokenType;
            viewModel.AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser;
            viewModel.AllowedCorsOrigins = string.Join(";", client.AllowedCorsOrigins.Select(x => x.Origin));
            viewModel.SetAllowedGrantTypes(client.AllowedGrantTypes.Select(x => x.GrantType).ToList());
            viewModel.AllowedScopes = string.Join(" ", client.AllowedScopes.Select(x => x.Scope));
            viewModel.AllowOfflineAccess = client.AllowOfflineAccess;
            viewModel.AllowPlainTextPkce = client.AllowPlainTextPkce;
            viewModel.AllowRememberConsent = client.AllowRememberConsent;
            viewModel.AlwaysIncludeUserClaimsInIdToken = client.AlwaysIncludeUserClaimsInIdToken;
            viewModel.AlwaysSendClientClaims = client.AlwaysSendClientClaims;
            viewModel.AuthorizationCodeLifetime = client.AuthorizationCodeLifetime;
            viewModel.BackChannelLogoutSessionRequired = client.BackChannelLogoutSessionRequired;
            viewModel.BackChannelLogoutUri = client.BackChannelLogoutUri;
            // viewModel.Claims = string.Join(" ", client.Claims.Select(x => x.Type));
            viewModel.ClientClaimsPrefix = client.ClientClaimsPrefix;
            viewModel.ClientId = client.ClientId;
            viewModel.ClientName = client.ClientName;
            viewModel.ClientUri = client.ClientUri;
            viewModel.ConsentLifetime = client.ConsentLifetime;
            viewModel.Description = client.Description;
            viewModel.DeviceCodeLifetime = client.DeviceCodeLifetime;
            viewModel.Enabled = client.Enabled;
            viewModel.EnableLocalLogin = client.EnableLocalLogin;
            viewModel.FrontChannelLogoutSessionRequired = client.FrontChannelLogoutSessionRequired;
            viewModel.FrontChannelLogoutUri = client.FrontChannelLogoutUri;
            viewModel.IdentityProviderRestrictions =
                string.Join(" ", client.IdentityProviderRestrictions.Select(x => x.Provider));
            viewModel.IdentityTokenLifetime = client.IdentityTokenLifetime;
            viewModel.IncludeJwtId = client.IncludeJwtId;
            viewModel.LogoUri = client.LogoUri;
            viewModel.PairWiseSubjectSalt = client.PairWiseSubjectSalt;
            viewModel.PostLogoutRedirectUris =
                string.Join(";", client.PostLogoutRedirectUris.Select(x => x.PostLogoutRedirectUri));
            // Properties
            viewModel.ProtocolType = client.ProtocolType;
            viewModel.RedirectUris = string.Join(";", client.RedirectUris.Select(x => x.RedirectUri));
            viewModel.RefreshTokenExpiration = (TokenExpiration) client.RefreshTokenExpiration;
            viewModel.RefreshTokenUsage = (TokenUsage) client.RefreshTokenUsage;
            viewModel.RequireClientSecret = client.RequireClientSecret;
            viewModel.ClientSecrets = string.Join(Environment.NewLine, client.ClientSecrets.Select(x => x.Value));
            viewModel.RequireConsent = client.RequireConsent;
            viewModel.RequirePkce = client.RequirePkce;
            viewModel.SlidingRefreshTokenLifetime = client.SlidingRefreshTokenLifetime;
            viewModel.UpdateAccessTokenClaimsOnRefresh = client.UpdateAccessTokenClaimsOnRefresh;
            viewModel.UserCodeType = client.UserCodeType;
            viewModel.UserSsoLifetime = client.UserSsoLifetime;

            ViewData["ReturnUrl"] = returnUrl;
            ViewData["RenderSecrets"] = true;

            return View("View", viewModel);
        }

        [HttpGet("{clientId}/detail")]
        public async Task<IActionResult> DetailAsync(int clientId)
        {
            var client = await _dbContext.Clients
                .Include(x => x.AllowedGrantTypes)
                .Include(x => x.RedirectUris)
                .Include(x => x.PostLogoutRedirectUris)
                .Include(x => x.AllowedScopes)
                .Include(x => x.ClientSecrets)
                .Include(x => x.Claims)
                .Include(x => x.IdentityProviderRestrictions)
                .Include(x => x.AllowedCorsOrigins)
                .Include(x => x.Properties)
                .AsNoTracking().FirstOrDefaultAsync(c => c.Id == clientId);
            if (client == null)
            {
                return NotFound();
            }

            var dict = new Dictionary<string, object>
            {
                {"Enabled", client.Enabled},
                {"ClientId", client.ClientId},
                {"ProtocolType", client.ProtocolType},
                {
                    "ClientSecrets",
                    client.ClientSecrets == null ? "" : string.Join("; ", client.ClientSecrets.Select(cs => cs.Value))
                },
                {"RequireClientSecret", client.RequireClientSecret},
                {"ClientName", client.ClientName},
                {"Description", client.Description},
                {"ClientUri", client.ClientUri},
                {"LogoUri", client.LogoUri},
                {"RequireConsent", client.RequireConsent},
                {"AllowRememberConsent", client.AllowRememberConsent},
                {"AlwaysIncludeUserClaimsInIdToken", client.AlwaysIncludeUserClaimsInIdToken},
                {
                    "AllowedGrantTypes", client.AllowedGrantTypes == null
                        ? ""
                        : string.Join("; ", client.AllowedGrantTypes.Select(agt => agt.GrantType))
                },
                {"RequirePkce", client.RequirePkce},
                {"AllowPlainTextPkce", client.AllowPlainTextPkce},
                {"AllowAccessTokensViaBrowser", client.AllowAccessTokensViaBrowser},
                {
                    "RedirectUris",
                    client.RedirectUris == null ? "" : string.Join("; ", client.RedirectUris.Select(t => t.RedirectUri))
                },
                {
                    "PostLogoutRedirectUris", client.PostLogoutRedirectUris == null
                        ? ""
                        : string.Join("; ", client.PostLogoutRedirectUris.Select(t => t.PostLogoutRedirectUri))
                },
                {"FrontChannelLogoutUri", client.FrontChannelLogoutUri},
                {"FrontChannelLogoutSessionRequired", client.FrontChannelLogoutSessionRequired},
                {"BackChannelLogoutUri", client.BackChannelLogoutUri},
                {"BackChannelLogoutSessionRequired", client.BackChannelLogoutSessionRequired},
                {"AllowOfflineAccess", client.AllowOfflineAccess},
                {
                    "AllowedScopes",
                    client.AllowedScopes == null ? "" : string.Join(" ", client.AllowedScopes.Select(t => t.Scope))
                },
                {"IdentityTokenLifetime", client.IdentityTokenLifetime},
                {"AccessTokenLifetime", client.AccessTokenLifetime},
                {"AuthorizationCodeLifetime", client.AuthorizationCodeLifetime},
                {"ConsentLifetime", client.ConsentLifetime},
                {"AbsoluteRefreshTokenLifetime", client.AbsoluteRefreshTokenLifetime},
                {"SlidingRefreshTokenLifetime", client.SlidingRefreshTokenLifetime},
                {"RefreshTokenUsage", client.RefreshTokenUsage},
                {"UpdateAccessTokenClaimsOnRefresh", client.UpdateAccessTokenClaimsOnRefresh},
                {"RefreshTokenExpiration", client.RefreshTokenExpiration},
                {"AccessTokenType", client.AccessTokenType == 0 ? "Jwt" : "Reference"},
                {"EnableLocalLogin", client.EnableLocalLogin},
                {
                    "IdentityProviderRestrictions", client.IdentityProviderRestrictions == null
                        ? ""
                        : string.Join("; ", client.IdentityProviderRestrictions.Select(t => t.Provider))
                },
                {"IncludeJwtId", client.IncludeJwtId},
                {"Claims", client.Claims == null ? "" : string.Join("; ", client.Claims.Select(t => t.Value))},
                {"AlwaysSendClientClaims", client.AlwaysSendClientClaims},
                {"ClientClaimsPrefix", client.ClientClaimsPrefix},
                {"PairWiseSubjectSalt", client.PairWiseSubjectSalt},
                {
                    "AllowedCorsOrigins", client.AllowedCorsOrigins == null
                        ? ""
                        : string.Join("; ", client.AllowedCorsOrigins.Select(t => t.Origin))
                },
                {
                    "Properties",
                    client.Properties == null ? "" : string.Join("; ", client.Properties.Select(t => t.Value))
                },
                {"Created", client.Created},
                {"UserSsoLifetime", client.UserSsoLifetime},
                {"UserCodeType", client.UserCodeType},
                {"DeviceCodeLifetime", client.DeviceCodeLifetime}
            };
            var sortDict = new Dictionary<string, object>();
            var keys = dict.Keys.ToList();
            keys.Sort();
            foreach (var key in keys)
            {
                sortDict.Add(key, dict[key]);
            }

            return new JsonResult(sortDict);
        }
    }
}