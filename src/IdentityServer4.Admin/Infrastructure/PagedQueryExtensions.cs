using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.Admin.Infrastructure
{
    public static class PagedQueryExtensions
    {
        public static async Task<PagedQueryResult<TEntity>> PagedQueryAsync<TEntity>(this IQueryable<TEntity> queryable,
            PagedQuery input,
            Expression<Func<TEntity, bool>> where = null) where TEntity : class
        {
            var result = new PagedQueryResult<TEntity>();

            var entities = where == null ? queryable : queryable.Where(where);

            result.Total = await entities.CountAsync();
            result.Page = input.Page ?? 1;
            result.Size = input.Size ?? 20;

            entities = entities.Skip((result.Page - 1) * result.Size).Take(result.Size);

            result.Result = result.Total == 0 ? new List<TEntity>() : await entities.ToListAsync();
            return result;
        }
    }
}