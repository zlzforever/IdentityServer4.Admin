using System;

namespace IdentityServer4.Admin.Infrastructure.Entity
{
    /// <summary>
    /// 数据模型接口
    /// </summary>
    public interface IEntity<TKey> : ICreationAudited, IModificationAudited where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// 获取 实体唯一标识，主键
        /// </summary>
        TKey Id { get; set; }

        bool IsTransient();
    }
}