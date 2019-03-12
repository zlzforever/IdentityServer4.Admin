using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers.Api
{
    [Route("api/identity-resource")]
    [Authorize(Roles = AdminConsts.AdminName)]
    [SecurityHeaders]
    public class IdentityResourceController : ControllerBase
    {
        private readonly IDbContext _dbContext;

        public IdentityResourceController(
            IDbContext dbContext,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _dbContext = dbContext;
        }

        [HttpPut("{id}/disable")]
        public async Task<IActionResult> DisableAsync(int id)
        {
            var resource = await _dbContext.IdentityResources.FirstOrDefaultAsync(x => x.Id == id);
            if (resource == null)
            {
                return BadRequest(new {Code = 400, Msg = "Resource not exists"});
            }

            resource.Enabled = false;
            _dbContext.IdentityResources.Update(resource);
            await _dbContext.SaveChangesAsync();
            return Ok(new {Code = 200, Msg = "Disable success"});
        }

        [HttpPut("{id}/enable")]
        public async Task<IActionResult> EnableAsync(int id)
        {
            var resource = await _dbContext.IdentityResources.FirstOrDefaultAsync(x => x.Id == id);
            if (resource == null)
            {
                return BadRequest(new {Code = 400, Msg = "Resource not exists"});
            }

            resource.Enabled = true;
            _dbContext.IdentityResources.Update(resource);
            await _dbContext.SaveChangesAsync();
            return Ok(new {Code = 200, Msg = "Enable success"});
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var resource = await _dbContext.IdentityResources.FirstOrDefaultAsync(x => x.Id == id);
            if (resource == null)
            {
                return BadRequest(new {Code = 400, Msg = "Resource not exists"});
            }

            var context = (DbContext) _dbContext;
            var transaction = context.Database.BeginTransaction();
            try
            {
                var claims = _dbContext.IdentityClaims.Where(x => x.IdentityResourceId == id);
                _dbContext.IdentityClaims.RemoveRange(claims);
                _dbContext.IdentityResources.Remove(resource);
                await _dbContext.SaveChangesAsync();
                transaction.Commit();
            }
            catch (Exception e)
            {
                Logger.LogError($"Delete identity resource failed: {e}");
                try
                {
                    transaction.Rollback();
                }
                catch (Exception te)
                {
                    Logger.LogError($"Rollback delete identity resource failed: {te}");
                }

                return StatusCode(500, new {Code = 500, Msg = "Delete identity resource failed"});
            }

            return Ok(new {Code = 200, Msg = "Delete success"});
        }
    }
}