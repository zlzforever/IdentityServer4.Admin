using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Infrastructure
{
    public class ProfileService : IProfileService
    {
        private readonly ILogger _logger;
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultProfileService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="userManager">用户管理器</param>
        public ProfileService(ILogger<DefaultProfileService> logger, UserManager<User> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        /// <summary>
        /// This method is called whenever claims about the user are requested (e.g. during token creation or via the userinfo endpoint)
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public virtual async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.AddRequestedClaims(context.Subject.Claims);

            var user = await _userManager.GetUserAsync(context.Subject);

            if (user == null)
            {
                throw new ArgumentException("User not exits");
            }

            var claims = new HashSet<string>();
            foreach (var resource in context.RequestedResources.IdentityResources)
            {
                foreach (var claim in resource.UserClaims)
                {
                    claims.Add(claim);
                }
            }

            context.IssuedClaims = context.IssuedClaims ?? new List<Claim>();

            foreach (var claim in claims)
            {
                switch (claim)
                {
                    case JwtClaimTypes.Name:
                    {
                        context.IssuedClaims.Add(context.Subject.FindFirst(JwtClaimTypes.Name));
                        continue;
                    }
                    case JwtClaimTypes.IdentityProvider:
                    {
                        context.IssuedClaims.Add(context.Subject.FindFirst(JwtClaimTypes.IdentityProvider));
                        continue;
                    }
                    case JwtClaimTypes.AuthenticationMethod:
                    {
                        context.IssuedClaims.Add(context.Subject.FindFirst(JwtClaimTypes.AuthenticationMethod));
                        continue;
                    }
                    case JwtClaimTypes.AuthenticationTime:
                    {
                        context.IssuedClaims.Add(context.Subject.FindFirst(JwtClaimTypes.AuthenticationTime));
                        continue;
                    }
                    case JwtClaimTypes.Role:
                    {
                        context.IssuedClaims.AddRange(context.Subject.FindAll(JwtClaimTypes.Role));
                        continue;
                    }
                    case JwtClaimTypes.FamilyName:
                    {
                        context.IssuedClaims.Add(new Claim(JwtClaimTypes.FamilyName, user.FamilyName ?? ""));
                        continue;
                    }
                    case JwtClaimTypes.GivenName:
                    {
                        context.IssuedClaims.Add(new Claim(JwtClaimTypes.GivenName, user.GivenName ?? ""));
                        continue;
                    }
                    case JwtClaimTypes.PhoneNumber:
                    {
                        context.IssuedClaims.Add(new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber ?? ""));
                        continue;
                    }
                    case JwtClaimTypes.Email:
                    {
                        context.IssuedClaims.Add(new Claim(JwtClaimTypes.Email, user.Email ?? ""));
                        continue;
                    }
                    case "office_phone":
                    {
                        context.IssuedClaims.Add(new Claim("office_phone", user.OfficePhone ?? ""));
                        continue;
                    }
                    case "title":
                    {
                        context.IssuedClaims.Add(new Claim("title", user.Title ?? ""));
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// This method gets called whenever identity server needs to determine if the user is valid or active (e.g. if the user's account has been deactivated since they logged in).
        /// (e.g. during token issuance or validation).
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public virtual Task IsActiveAsync(IsActiveContext context)
        {
            _logger.LogDebug("IsActive called from: {caller}", context.Caller);

            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}