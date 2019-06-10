using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.Infrastructure.Entity;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer4.Admin.Entities
{
    /// <summary>
    /// 用户
    /// </summary>
    public class User : IdentityUser<Guid>, IEntity<Guid>
    {
        /// <summary>
        /// 名
        /// </summary>
        [StringLength(256)]
        public string GivenName { get; set; }

        /// <summary>
        /// 姓
        /// </summary>
        [StringLength(256)]
        public string FamilyName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(256)]
        public string NickName { get; set; }
        
        /// <summary>
        /// 标语
        /// </summary>
        [StringLength(256)]
        public string Slogan { get; set; }
        
        /// <summary>
        /// 个人网址
        /// </summary>
        [StringLength(500)]
        public string WebSite { get; set; }
        
        /// <summary>
        /// 所在地
        /// </summary>
        [StringLength(256)]
        public string Location { get; set; }
        
        /// <summary>
        /// 头像
        /// </summary>
        [StringLength(256)]
        public string Icon { get; set; }
        
        /// <summary>
        /// 职位
        /// </summary>
        [StringLength(256)]
        public string Title { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public Sex Sex { get; set; }

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
        /// 公司电话
        /// </summary>
        [StringLength(256)]
        public string OfficePhone { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [StringLength(256)]
        [Required]
        [ProtectedPersonalData]
        public override string UserName { get; set; }

        /// <summary>
        /// The normalized userName
        /// </summary>
        [StringLength(256)]
        public override string NormalizedUserName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [ProtectedPersonalData]
        [StringLength(256)]
        public override string Email { get; set; }

        /// <summary>
        /// Gets or sets the normalized email address for this user.
        /// </summary>
        [StringLength(256)]
        public override string NormalizedEmail { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if a user has confirmed their email address.
        /// </summary>
        /// <value>True if the email address has been confirmed, otherwise false.</value>
        [PersonalData]
        public override bool EmailConfirmed { get; set; }

        /// <summary>
        /// Gets or sets a salted and hashed representation of the password for this user.
        /// </summary>
        [StringLength(512)]
        public override string PasswordHash { get; set; }

        /// <summary>
        /// A random value that must change whenever a users credentials change (password changed, login removed)
        /// </summary>
        [StringLength(36)]
        public override string SecurityStamp { get; set; }

        /// <summary>
        /// A random value that must change whenever a user is persisted to the store
        /// </summary>
        [StringLength(36)]
        public override string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>Gets or sets a telephone number for the user.</summary>
        [ProtectedPersonalData]
        [StringLength(256)]
        public override string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if a user has confirmed their telephone address.
        /// </summary>
        /// <value>True if the telephone number has been confirmed, otherwise false.</value>
        [PersonalData]
        public override bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if two factor authentication is enabled for this user.
        /// </summary>
        /// <value>True if 2fa is enabled, otherwise false.</value>
        [PersonalData]
        public override bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when any user lockout ends.
        /// </summary>
        /// <remarks>A value in the past means the user is not locked out.</remarks>
        public override DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if the user could be locked out.
        /// </summary>
        /// <value>True if the user could be locked out, otherwise false.</value>
        public override bool LockoutEnabled { get; set; }

        /// <summary>
        /// Gets or sets the number of failed login attempts for the current user.
        /// </summary>
        public override int AccessFailedCount { get; set; }

        /// <summary>
        /// Checks if this entity is transient (it has not an Id).
        /// </summary>
        /// <returns>True, if this entity is transient</returns>
        public virtual bool IsTransient()
        {
            if (EqualityComparer<Guid>.Default.Equals(Id, default))
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
}