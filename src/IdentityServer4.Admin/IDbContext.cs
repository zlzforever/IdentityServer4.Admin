using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Admin.Entities;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.Admin
{
    public interface IDbContext : IDbContext<User, Role, Guid, IdentityUserClaim<Guid>, IdentityUserRole<Guid>,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
    }

    public interface
        IDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim,
            TUserToken> : IConfigurationDbContext, IPersistedGrantDbContext
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TUserToken : IdentityUserToken<TKey>
        where TRole : IdentityRole<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
    {
        DbSet<TUser> Users { get; set; }

        DbSet<TUserClaim> UserClaims { get; set; }

        DbSet<TUserLogin> UserLogins { get; set; }

        DbSet<TUserToken> UserTokens { get; set; }

        DbSet<TUserRole> UserRoles { get; set; }

        DbSet<TRole> Roles { get; set; }

        DbSet<TRoleClaim> RoleClaims { get; set; }

        DbSet<IdentityClaim> IdentityClaims { get; set; }

        DbSet<ApiResourceClaim> ApiResourceClaims { get; set; }

        DbSet<ApiScope> ApiScopes { get; set; }

        DbSet<ApiScopeClaim> ApiScopeClaims { get; set; }

        DbSet<ClientSecret> ClientSecrets { get; set; }

        DbSet<ClientGrantType> ClientGrantTypes { get; set; }
        
        DbSet<ClientRedirectUri> ClientRedirectUris { get; set; }
        
        DbSet<ClientPostLogoutRedirectUri> ClientPostLogoutRedirectUris { get; set; }
        
        DbSet<ClientScope> ClientScopes { get; set; }
        
        DbSet<ClientIdPRestriction> ClientIdPRestrictions { get; set; }
        
        DbSet<ClientCorsOrigin> ClientCorsOrigins { get; set; }

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}