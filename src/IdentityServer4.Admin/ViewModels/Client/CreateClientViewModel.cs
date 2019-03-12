using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IdentityServer4.Models;

namespace IdentityServer4.Admin.ViewModels.Client
{
    public class CreateClientViewModel
    {
        /// <summary>
        /// Unique ID of the client
        /// </summary>
        [Required]
        [StringLength(200)]
        public string ClientId { get; set; }

        /// <summary>
        /// Client display name (used for logging and consent screen)
        /// </summary>
        [Required]
        [StringLength(200)]
        public string ClientName { get; set; }

        /// <summary>
        /// Specifies the allowed grant types (legal combinations of AuthorizationCode, Implicit, Hybrid, ResourceOwner, ClientCredentials).
        /// </summary>
        [Required]
        public IdentityServer4.Admin.Infrastructure.GrantTypes AllowedGrantTypes { get; set; }

        /// <summary>
        /// Controls whether access tokens are transmitted via the browser for this client (defaults to <c>false</c>).
        /// This can prevent accidental leakage of access tokens when multiple response types are allowed.
        /// </summary>
        /// <value>
        /// <c>true</c> if access tokens can be transmitted via the browser; otherwise, <c>false</c>.
        /// </value>
        public bool AllowAccessTokensViaBrowser { get; set; } = false;

        /// <summary>
        /// Gets or sets the allowed CORS origins for JavaScript clients.
        /// </summary>
        /// <value>
        /// The allowed CORS origins.
        /// </value>
        [Required]
        [StringLength(2000)]
        public string AllowedCorsOrigins { get; set; }

        /// <summary>
        /// Specifies allowed URIs to return tokens or authorization codes to
        /// </summary>
        [Required]
        [StringLength(2000)]
        public string RedirectUris { get; set; }

        /// <summary>
        /// Specifies allowed URIs to redirect to after logout
        /// </summary>
        [StringLength(2000)]
        public string PostLogoutRedirectUris { get; set; }

        public bool RequireConsent { get; set; }

        [Required] [StringLength(2000)] public string AllowedScopes { get; set; }

        /// <summary>
        /// Gets or sets the protocol type.
        /// </summary>
        /// <value>
        /// The protocol type.
        /// </value>
        [StringLength(200)]
        public string ProtocolType { get; set; } = IdentityServerConstants.ProtocolTypes.OpenIdConnect;

        /// <summary>
        /// If set to false, no client secret is needed to request tokens at the token endpoint (defaults to <c>true</c>)
        /// </summary>
        public bool RequireClientSecret { get; set; } = true;

        /// <summary>
        /// Description of the 
        /// </summary>
        [StringLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// URI to further information about client (used on consent screen)
        /// </summary>
        [StringLength(2000)]
        [Url]
        public string ClientUri { get; set; }

        /// <summary>
        /// URI to client logo (used on consent screen)
        /// </summary>
        [StringLength(2000)]
        [Url]
        public string LogoUri { get; set; }

        /// <summary>
        /// Specifies whether user can choose to store consent decisions (defaults to <c>true</c>)
        /// </summary>
        public bool AllowRememberConsent { get; set; } = true;

        /// <summary>
        /// Specifies whether a proof key is required for authorization code based token requests (defaults to <c>false</c>).
        /// </summary>
        public bool RequirePkce { get; set; } = false;

        /// <summary>
        /// Specifies whether a proof key can be sent using plain method (not recommended and defaults to <c>false</c>.)
        /// </summary>
        public bool AllowPlainTextPkce { get; set; } = false;

        /// <summary>
        /// Specifies logout URI at client for HTTP front-channel based logout.
        /// </summary>
        [Url]
        [StringLength(2000)]
        public string FrontChannelLogoutUri { get; set; }

        /// <summary>
        /// Specifies is the user's session id should be sent to the FrontChannelLogoutUri. Defaults to <c>true</c>.
        /// </summary>
        public bool FrontChannelLogoutSessionRequired { get; set; } = true;

        /// <summary>
        /// Specifies logout URI at client for HTTP back-channel based logout.
        /// </summary>
        [Url]
        [StringLength(2000)]
        public string BackChannelLogoutUri { get; set; }

        /// <summary>
        /// Specifies is the user's session id should be sent to the BackChannelLogoutUri. Defaults to <c>true</c>.
        /// </summary>
        public bool BackChannelLogoutSessionRequired { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether [allow offline access]. Defaults to <c>false</c>.
        /// </summary>
        public bool AllowOfflineAccess { get; set; } = false;

        /// <summary>
        /// When requesting both an id token and access token, should the user claims always be added to the id token instead of requring the client to use the userinfo endpoint.
        /// Defaults to <c>false</c>.
        /// </summary>
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; } = false;

        /// <summary>
        /// Lifetime of identity token in seconds (defaults to 300 seconds / 5 minutes)
        /// </summary>
        [Range(10, int.MaxValue)]
        public int IdentityTokenLifetime { get; set; } = 300;

        /// <summary>
        /// Lifetime of access token in seconds (defaults to 3600 seconds / 1 hour)
        /// </summary>
        [Range(10, int.MaxValue)]
        public int AccessTokenLifetime { get; set; } = 3600;

        /// <summary>
        /// Lifetime of authorization code in seconds (defaults to 300 seconds / 5 minutes)
        /// </summary>
        [Range(10, int.MaxValue)]
        public int AuthorizationCodeLifetime { get; set; } = 300;

        /// <summary>
        /// Maximum lifetime of a refresh token in seconds. Defaults to 2592000 seconds / 30 days
        /// </summary>
        [Range(10, int.MaxValue)]
        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;

        /// <summary>
        /// Sliding lifetime of a refresh token in seconds. Defaults to 1296000 seconds / 15 days
        /// </summary>
        [Range(10, int.MaxValue)]
        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;

        /// <summary>
        /// Lifetime of a user consent in seconds. Defaults to null (no expiration)
        /// </summary>
        public int? ConsentLifetime { get; set; } = null;

        /// <summary>
        /// ReUse: the refresh token handle will stay the same when refreshing tokens
        /// OneTime: the refresh token handle will be updated when refreshing tokens
        /// </summary>
        public TokenUsage RefreshTokenUsage { get; set; } = TokenUsage.OneTimeOnly;

        /// <summary>
        /// Gets or sets a value indicating whether the access token (and its claims) should be updated on a refresh token request.
        /// Defaults to <c>false</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> if the token should be updated; otherwise, <c>false</c>.
        /// </value>
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; } = false;

        /// <summary>
        /// Absolute: the refresh token will expire on a fixed point in time (specified by the AbsoluteRefreshTokenLifetime)
        /// Sliding: when refreshing the token, the lifetime of the refresh token will be renewed (by the amount specified in SlidingRefreshTokenLifetime). The lifetime will not exceed AbsoluteRefreshTokenLifetime.
        /// </summary>        
        public TokenExpiration RefreshTokenExpiration { get; set; } = TokenExpiration.Absolute;

        /// <summary>
        /// Specifies whether the access token is a reference token or a self contained JWT token (defaults to Jwt).
        /// </summary>
        public AccessTokenType AccessTokenType { get; set; } = AccessTokenType.Jwt;

        /// <summary>
        /// Gets or sets a value indicating whether the local login is allowed for this client.  Defaults to <c>true</c>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if local logins are enabled; otherwise, <c>false</c>.
        /// </value>
        public bool EnableLocalLogin { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether JWT access tokens should include an identifier. Defaults to <c>false</c>.
        /// </summary>
        /// <value>
        /// <c>true</c> to add an id; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeJwtId { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether client claims should be always included in the access tokens - or only for client credentials flow.
        /// Defaults to <c>false</c>
        /// </summary>
        /// <value>
        /// <c>true</c> if claims should always be sent; otherwise, <c>false</c>.
        /// </value>
        public bool AlwaysSendClientClaims { get; set; } = false;

        /// <summary>
        /// Gets or sets a value to prefix it on client claim types. Defaults to <c>client_</c>.
        /// </summary>
        /// <value>
        /// Any non empty string if claims should be prefixed with the value; otherwise, <c>null</c>.
        /// </value>
        [StringLength(200)]
        public string ClientClaimsPrefix { get; set; } = "client_";

        /// <summary>
        /// Gets or sets a salt value used in pair-wise subjectId generation for users of this client. 
        /// </summary>
        [StringLength(200)]
        public string PairWiseSubjectSalt { get; set; }

        /// <summary>
        /// The maximum duration (in seconds) since the last time the user authenticated.
        /// </summary>
        public int? UserSsoLifetime { get; set; }

        /// <summary>
        /// Gets or sets the type of the device flow user code.
        /// </summary>
        /// <value>
        /// The type of the device flow user code.
        /// </value>
        [StringLength(100)]
        public string UserCodeType { get; set; }

        /// <summary>
        /// Gets or sets the device code lifetime.
        /// </summary>
        /// <value>
        /// The device code lifetime.
        /// </value>
        public int DeviceCodeLifetime { get; set; } = 300;
        
        public string IdentityProviderRestrictions { get; set; }

        public ICollection<string> GetAllowedGrantTypes()
        {
            switch (AllowedGrantTypes)
            {
                case IdentityServer4.Admin.Infrastructure.GrantTypes.Code:
                {
                    return Models.GrantTypes.Code;
                }
                case IdentityServer4.Admin.Infrastructure.GrantTypes.Hybrid:
                {
                    return Models.GrantTypes.Hybrid;
                }
                case IdentityServer4.Admin.Infrastructure.GrantTypes.Implicit:
                {
                    return Models.GrantTypes.Implicit;
                }
                case IdentityServer4.Admin.Infrastructure.GrantTypes.ClientCredentials:
                {
                    return Models.GrantTypes.ClientCredentials;
                }
                case IdentityServer4.Admin.Infrastructure.GrantTypes.DeviceFlow:
                {
                    return Models.GrantTypes.DeviceFlow;
                }
                case IdentityServer4.Admin.Infrastructure.GrantTypes.ResourceOwnerPassword:
                {
                    return Models.GrantTypes.ResourceOwnerPassword;
                }
                case IdentityServer4.Admin.Infrastructure.GrantTypes.CodeAndClientCredentials:
                {
                    return Models.GrantTypes.CodeAndClientCredentials;
                }
                case IdentityServer4.Admin.Infrastructure.GrantTypes.HybridAndClientCredentials:
                {
                    return Models.GrantTypes.HybridAndClientCredentials;
                }
                case IdentityServer4.Admin.Infrastructure.GrantTypes.ImplicitAndClientCredentials:
                {
                    return Models.GrantTypes.ImplicitAndClientCredentials;
                }
                case IdentityServer4.Admin.Infrastructure.GrantTypes.ResourceOwnerPasswordAndClientCredentials:
                {
                    return Models.GrantTypes.ResourceOwnerPasswordAndClientCredentials;
                }
            }

            throw new ArgumentException("不支持的授权类型");
        }
    }
}