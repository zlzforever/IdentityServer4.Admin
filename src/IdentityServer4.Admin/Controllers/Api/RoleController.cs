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
    [Route("api/[controller]")]
    [Authorize(Roles = AdminConsts.AdminName)]
    [SecurityHeaders]
    public class RoleController : ControllerBase
    {
        private readonly IDbContext _dbContext;

        public RoleController(
            IDbContext dbContext,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _dbContext = dbContext;
        }

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteAsync(Guid roleId)
        {
            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
            if (role == null)
            {
                return BadRequest(new {Code = 400, Msg = "Role not exists"});
            }


            if (role.Name == AdminConsts.AdminName)
            {
                return BadRequest(new {Code = 500, Msg = "Delete admin is not allowed"});
            }

            _dbContext.UserRoles.RemoveRange(_dbContext.UserRoles.Where(ur => ur.RoleId == roleId));
            _dbContext.Roles.Remove(role);
            await _dbContext.SaveChangesAsync();
            return Ok(new {Code = 200, Msg = "success"});
        }
    }
}