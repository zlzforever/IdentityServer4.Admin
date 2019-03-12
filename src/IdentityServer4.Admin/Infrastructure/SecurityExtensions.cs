using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.Admin.Infrastructure
{
    public static class SecurityExtensions
    {
        public static string ProtectPersonalData(this IServiceProvider service, string data, IdentityOptions options)
        {
            if (options.Stores.ProtectPersonalData)
                return service.GetRequiredService<ILookupProtector>()
                    .Protect(service.GetRequiredService<ILookupProtectorKeyRing>().CurrentKeyId, data);
            return data;
        }
    }
}