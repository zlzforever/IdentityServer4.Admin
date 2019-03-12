using System.Collections.Generic;

namespace IdentityServer4.Admin.Infrastructure
{
    public class PagedQueryResult<TEntity>
    {
        /// <summary>
        /// 总计
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 当前页数 
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 每页数据量 
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 当前页结果
        /// </summary>
        public List<TEntity> Result { get; set; }

        public PagedQueryResult ToResult(object dtoResult)
        {
            return new PagedQueryResult
            {
                Total = Total,
                Page = Page,
                Size = Size,
                Result = dtoResult
            };
        }
    }

    public class PagedQueryResult
    {
        /// <summary>
        /// 总计
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 当前页数 
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 每页数据量 
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 当前页结果
        /// </summary>
        public object Result { get; set; }
    }
}