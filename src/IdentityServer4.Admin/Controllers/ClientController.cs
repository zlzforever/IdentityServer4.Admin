using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.ViewModels.Client;
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


        [HttpGet("{clientId}")]
        public async Task<IActionResult> ViewAsync(int clientId, string returnUrl)
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
            var dic = new Dictionary<string, object>();
            dic.Add("Enabled", client.Enabled);
            dic.Add("ClientId", client.ClientId);
            dic.Add("ProtocolType", client.ProtocolType);
            dic.Add("ClientSecrets",
                client.ClientSecrets == null ? "" : string.Join("; ", client.ClientSecrets.Select(cs => cs.Value)));
            dic.Add("RequireClientSecret", client.RequireClientSecret);
            dic.Add("ClientName", client.ClientName);
            dic.Add("Description", client.Description);
            dic.Add("ClientUri", client.ClientUri);
            dic.Add("LogoUri", client.LogoUri);
            dic.Add("RequireConsent", client.RequireConsent);
            dic.Add("AllowRememberConsent", client.AllowRememberConsent);
            dic.Add("AlwaysIncludeUserClaimsInIdToken", client.AlwaysIncludeUserClaimsInIdToken);
            dic.Add("AllowedGrantTypes",
                client.AllowedGrantTypes == null
                    ? ""
                    : string.Join("; ", client.AllowedGrantTypes.Select(agt => agt.GrantType)));
            dic.Add("RequirePkce", client.RequirePkce);
            dic.Add("AllowPlainTextPkce", client.AllowPlainTextPkce);
            dic.Add("AllowAccessTokensViaBrowser", client.AllowAccessTokensViaBrowser);
            dic.Add("RedirectUris",
                client.RedirectUris == null ? "" : string.Join("; ", client.RedirectUris.Select(t => t.RedirectUri)));
            dic.Add("PostLogoutRedirectUris",
                client.PostLogoutRedirectUris == null
                    ? ""
                    : string.Join("; ", client.PostLogoutRedirectUris.Select(t => t.PostLogoutRedirectUri)));
            dic.Add("FrontChannelLogoutUri", client.FrontChannelLogoutUri);
            dic.Add("FrontChannelLogoutSessionRequired", client.FrontChannelLogoutSessionRequired);
            dic.Add("BackChannelLogoutUri", client.BackChannelLogoutUri);
            dic.Add("BackChannelLogoutSessionRequired", client.BackChannelLogoutSessionRequired);
            dic.Add("AllowOfflineAccess", client.AllowOfflineAccess);
            dic.Add("AllowedScopes",
                client.AllowedScopes == null ? "" : string.Join(" ", client.AllowedScopes.Select(t => t.Scope)));
            dic.Add("IdentityTokenLifetime", client.IdentityTokenLifetime);
            dic.Add("AccessTokenLifetime", client.AccessTokenLifetime);
            dic.Add("AuthorizationCodeLifetime", client.AuthorizationCodeLifetime);
            dic.Add("ConsentLifetime", client.ConsentLifetime);
            dic.Add("AbsoluteRefreshTokenLifetime", client.AbsoluteRefreshTokenLifetime);
            dic.Add("SlidingRefreshTokenLifetime", client.SlidingRefreshTokenLifetime);
            dic.Add("RefreshTokenUsage", client.RefreshTokenUsage);
            dic.Add("UpdateAccessTokenClaimsOnRefresh", client.UpdateAccessTokenClaimsOnRefresh);
            dic.Add("RefreshTokenExpiration", client.RefreshTokenExpiration);
            dic.Add("AccessTokenType", client.AccessTokenType == 0 ? "Jwt" : "Reference");
            dic.Add("EnableLocalLogin", client.EnableLocalLogin);
            dic.Add("IdentityProviderRestrictions",
                client.IdentityProviderRestrictions == null
                    ? ""
                    : string.Join("; ", client.IdentityProviderRestrictions.Select(t => t.Provider)));
            dic.Add("IncludeJwtId", client.IncludeJwtId);
            dic.Add("Claims", client.Claims == null ? "" : string.Join("; ", client.Claims.Select(t => t.Value)));
            dic.Add("AlwaysSendClientClaims", client.AlwaysSendClientClaims);
            dic.Add("ClientClaimsPrefix", client.ClientClaimsPrefix);
            dic.Add("PairWiseSubjectSalt", client.PairWiseSubjectSalt);
            dic.Add("AllowedCorsOrigins",
                client.AllowedCorsOrigins == null
                    ? ""
                    : string.Join("; ", client.AllowedCorsOrigins.Select(t => t.Origin)));
            dic.Add("Properties",
                client.Properties == null ? "" : string.Join("; ", client.Properties.Select(t => t.Value)));
            dic.Add("Created", client.Created);
            dic.Add("UserSsoLifetime", client.UserSsoLifetime);
            dic.Add("UserCodeType", client.UserCodeType);
            dic.Add("DeviceCodeLifetime", client.DeviceCodeLifetime);

            ViewData["ReturnUrl"] = returnUrl;

            return View("View", dic);
        }
    }
}