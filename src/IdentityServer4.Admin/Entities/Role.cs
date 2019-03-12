using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IdentityServer4.Admin.Infrastructure.Entity;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer4.Admin.Entities
{
    /// <summary>
    /// 角色
    /// </summary>
    public class Role<TKey> : IdentityRole<TKey>, IEntity<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// 角色名
        /// </summary>
        [StringLength(256)]
        [Required]
        public override string Name { get; set; }

        /// <summary>
        /// 标准化后的角色名
        /// </summary>
        [StringLength(256)]
        [Required]
        public override string NormalizedName { get; set; }

        /// <summary>
        /// A random value that should change whenever a role is persisted to the store
        /// </summary>
        [StringLength(36)]
        public override string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 角色描述
        /// </summary>
        [StringLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Last modification date of this entity.
        /// </summary>
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// Last modifier user of this entity.
        /// </summary>
        [StringLength(256)]
        public string LastModifierUserId { get; set; }

        /// <summary>
        /// Creation time of this entity.
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Creator of this entity.
        /// </summary>
        [StringLength(256)]
        public string CreatorUserId { get; set; }

        /// <summary>
        /// Checks if this entity is transient (it has not an Id).
        /// </summary>
        /// <returns>True, if this entity is transient</returns>
        public virtual bool IsTransient()
        {
            if (EqualityComparer<TKey>.Default.Equals(Id, default))
            {
                return true;
            }

            //Workaround for EF Core since it sets int/long to min value when attaching to dbcontext
            if (typeof(Guid) == typeof(int))
            {
                return Convert.ToInt32(Id) <= 0;
            }

            if (typeof(Guid) == typeof(long))
            {
                return Convert.ToInt64(Id) <= 0;
            }

            return false;
        }
    }

    /// <summary>
    /// 角色
    /// </summary>
    public class Role : Role<Guid>
    {
    }
}