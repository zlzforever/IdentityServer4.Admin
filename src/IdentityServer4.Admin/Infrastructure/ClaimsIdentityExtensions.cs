using System.Security.Claims;
using System.Security.Principal;
using IdentityModel;

namespace IdentityServer4.Admin.Infrastructure
{
    /// <summary>
    /// <see cref="ClaimsIdentity"/>扩展操作类
    /// </summary>
    public static class ClaimsIdentityExtensions
    {
        /// <summary>
        /// 获取用户ID
        /// </summary>
        public static string GetUserId(this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));
            if (!(identity is ClaimsIdentity claimsIdentity))
            {
                return null;
            }

            return claimsIdentity.FindFirst(JwtClaimTypes.Subject)?.Value;
        }

        /// <summary>
        /// 获取用户ID
        /// </summary>
        public static T GetUserId<T>(this IIdentity identity)
        {
            Check.NotNull(identity, nameof(identity));
            if (!(identity is ClaimsIdentity claimsIdentity))
            {
                return default;
            }

            var subject = claimsIdentity.FindFirst(JwtClaimTypes.Subject);

            return subject == null ? default : subject.Value.CastTo<T>();
        }
    }
}