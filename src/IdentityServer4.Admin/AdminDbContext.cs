using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.Infrastructure.Entity;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace IdentityServer4.Admin
{
    public class AdminDbContext : IdentityDbContext<User, Role, Guid>,
        IDbContext,
        IDesignTimeDbContextFactory<AdminDbContext>
    {
        private readonly ConfigurationStoreOptions _configurationStoreOptions;
        private readonly OperationalStoreOptions _operationalStoreOptions;

        /// <summary>
        /// Gets or sets the clients.
        /// </summary>
        /// <value>
        /// The clients.
        /// </value>
        public DbSet<Client> Clients { get; set; }

        /// <summary>
        /// Gets or sets the identity resources.
        /// </summary>
        /// <value>
        /// The identity resources.
        /// </value>
        public DbSet<IdentityResource> IdentityResources { get; set; }

        /// <summary>
        /// Gets or sets the API resources.
        /// </summary>
        /// <value>
        /// The API resources.
        /// </value>
        public DbSet<ApiResource> ApiResources { get; set; }

        /// <summary>
        /// Gets or sets the persisted grants.
        /// </summary>
        /// <value>
        /// The persisted grants.
        /// </value>
        public DbSet<PersistedGrant> PersistedGrants { get; set; }

        /// <summary>
        /// Gets or sets the device codes.
        /// </summary>
        /// <value>
        /// The device codes.
        /// </value>
        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

        public DbSet<IdentityClaim> IdentityClaims { get; set; }

        public DbSet<ApiResourceClaim> ApiResourceClaims { get; set; }

        public DbSet<ApiScope> ApiScopes { get; set; }

        public DbSet<ApiScopeClaim> ApiScopeClaims { get; set; }

        public DbSet<ClientSecret> ClientSecrets { get; set; }

        public DbSet<ClientGrantType> ClientGrantTypes { get; set; }
        public DbSet<ClientRedirectUri> ClientRedirectUris { get; set; }
        public DbSet<ClientPostLogoutRedirectUri> ClientPostLogoutRedirectUris { get; set; }
        public DbSet<ClientScope> ClientScopes { get; set; }
        public DbSet<ClientIdPRestriction> ClientIdPRestrictions { get; set; }
        public DbSet<ClientCorsOrigin> ClientCorsOrigins { get; set; }

        public AdminDbContext()
        {
        }

        public AdminDbContext(DbContextOptions<AdminDbContext> options,
            ConfigurationStoreOptions storeOptions,
            OperationalStoreOptions operationalStoreOptions)
            : base(options)
        {
            _configurationStoreOptions = storeOptions;
            _operationalStoreOptions = operationalStoreOptions;
        }

        public AdminDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AdminDbContext>();
            builder.UseSqlServer(GetConnectionString(args.Length > 0 ? args[0] : "appsettings.json"));

            return new AdminDbContext(builder.Options, new ConfigurationStoreOptions(), new OperationalStoreOptions());
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ConfigureClientContext(_configurationStoreOptions);
            builder.ConfigureResourcesContext(_configurationStoreOptions);
            builder.ConfigurePersistedGrantContext(_operationalStoreOptions);

            builder.Entity<ApiResource>().HasIndex(p => p.Name).IsUnique();

            builder.Entity<User>().HasIndex(u => u.CreationTime);

            base.OnModelCreating(builder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userId = this.GetService<IHttpContextAccessor>()?.HttpContext?.User?.Identity?.GetUserId();

            foreach (var entry in ChangeTracker.Entries().ToList())
            {
                ApplyConcepts(entry, userId);
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            var userId = this.GetService<IHttpContextAccessor>()?.HttpContext?.User?.Identity?.GetUserId();

            foreach (var entry in ChangeTracker.Entries().ToList())
            {
                ApplyConcepts(entry, userId);
            }

            return base.SaveChanges();
        }

        int IConfigurationDbContext.SaveChanges() => base.SaveChanges();

        Task<int> IConfigurationDbContext.SaveChangesAsync() => base.SaveChangesAsync();

        Task<int> IPersistedGrantDbContext.SaveChangesAsync() => base.SaveChangesAsync();

        int IPersistedGrantDbContext.SaveChanges() => base.SaveChanges();

        protected virtual void ApplyConcepts(EntityEntry entry, string userId)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    ApplyConceptsForAddedEntity(entry, userId);
                    break;
                case EntityState.Modified:
                    ApplyConceptsForModifiedEntity(entry, userId);
                    break;
//                case EntityState.Deleted:
//                    ApplyConceptsForDeletedEntity(entry, userId);
//                    break;
            }
        }

        protected virtual void ApplyConceptsForAddedEntity(EntityEntry entry, string userId)
        {
            CheckAndSetId(entry);

            var creationAudited = entry.Entity as ICreationAudited;
            if (creationAudited == null)
            {
                return;
            }

            if (creationAudited.CreationTime == default)
            {
                creationAudited.CreationTime = DateTime.Now;
            }

            if (creationAudited.CreatorUserId != null)
            {
                return;
            }

            //Finally, set CreatorUserId!
            creationAudited.CreatorUserId = userId;
        }

        protected virtual void ApplyConceptsForModifiedEntity(EntityEntry entry, string userId)
        {
            var modificationAudited = entry.Entity as IModificationAudited;
            if (modificationAudited == null)
            {
                return;
            }

            modificationAudited.LastModificationTime = DateTime.Now;
            modificationAudited.LastModifierUserId = userId;
        }

        protected virtual void SetDeletionAuditProperties(object entry, string userId)
        {
            var softDelete = entry as ISoftDelete;
            if (softDelete == null)
            {
                return;
            }

            softDelete.DeletionTime = DateTime.Now;
            softDelete.DeleterUserId = userId;
        }

        protected virtual void ApplyConceptsForDeletedEntity(EntityEntry entry, string userId)
        {
            CancelDeletionForSoftDelete(entry);
            SetDeletionAuditProperties(entry.Entity, userId);
        }

        protected virtual void CancelDeletionForSoftDelete(EntityEntry entry)
        {
            if (!(entry.Entity is ISoftDelete))
            {
                return;
            }

            entry.Reload();
            entry.State = EntityState.Modified;
            entry.Entity.As<ISoftDelete>().IsDeleted = true;
        }

        protected virtual void CheckAndSetId(EntityEntry entry)
        {
            //Set GUID Ids
            if (entry.Entity is IEntity<Guid> entity && entity.Id == Guid.Empty)
            {
                var idPropertyEntry = entry.Property("Id");

                if (idPropertyEntry != null && idPropertyEntry.Metadata.ValueGenerated == ValueGenerated.Never)
                {
                    entity.Id = CombGuid.NewGuid();
                }
            }
        }

        private string GetConnectionString(string config)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile(config, optional: false);

            var configuration = builder.Build();
            return configuration["ConnectionString"];
        }
    }
}