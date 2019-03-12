using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers.Api
{
    public partial class ApiResourceController
    {
        [HttpDelete("{resourceId}/scope/{id}")]
        public async Task<IActionResult> DeleteAsync(int resourceId, int id)
        {
            var resource = await _dbContext.ApiResources.FirstOrDefaultAsync(x => x.Id == resourceId);
            if (resource == null)
            {
                return BadRequest(new {Code = 400, Msg = "Resource not exists"});
            }

            var scope = await _dbContext.ApiScopes.FirstOrDefaultAsync(x => x.Id == id);
            if (scope == null)
            {
                return BadRequest(new {Code = 400, Msg = "Scope not exists"});
            }

            if (scope.ApiResourceId != resourceId)
            {
                return BadRequest(new {Code = 400, Msg = "Invalid data"});
            }


            var context = (DbContext) _dbContext;
            var transaction = context.Database.BeginTransaction();
            try
            {
                var claims = _dbContext.ApiScopeClaims.Where(x => x.ApiScopeId == id);
                _dbContext.ApiScopeClaims.RemoveRange(claims);
                _dbContext.ApiScopes.Remove(scope);
                await _dbContext.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception e)
            {
                Logger.LogError($"Delete api scope failed: {e}");
                try
                {
                    transaction.Rollback();
                }
                catch (Exception te)
                {
                    Logger.LogError($"Rollback delete api scope failed: {te}");
                }

                return StatusCode(500, new {Code = 500, Msg = "Delete api scope failed"});
            }

            return Ok(new {Code = 200, Msg = "Delete success"});
        }
    }
}