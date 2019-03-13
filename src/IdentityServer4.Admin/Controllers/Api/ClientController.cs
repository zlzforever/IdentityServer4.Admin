using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers.Api
{
    [Route("api/[controller]")]
    [Authorize(Roles = AdminConsts.AdminName)]
    [SecurityHeaders]
    public class ClientController : ControllerBase
    {
        private readonly IDbContext _dbContext;

        public ClientController(IDbContext dbContext,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _dbContext = dbContext;
        }


        [HttpDelete("{clientId}")]
        public async Task<IActionResult> DeleteAsync(int clientId)
        {
            var client = await _dbContext.Clients.FirstOrDefaultAsync(u => u.Id == clientId);
            if (client == null)
            {
                return NotFound(new {Code = 400, Msg = "Client not exists"});
            }

            // 会自动级联删除相关数据
            _dbContext.Clients.Remove(client);
            await _dbContext.SaveChangesAsync();
            return Ok(new {Code = 200});
        }
    }
}