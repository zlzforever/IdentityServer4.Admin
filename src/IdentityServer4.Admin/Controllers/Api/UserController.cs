using System;
using System.Threading.Tasks;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers.Api
{
    [Route("api/[controller]")]
    [Authorize(Roles = AdminConsts.AdminName)]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public UserController(UserManager<User> userManager,
            RoleManager<Role> roleManager,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #region User       

        [HttpPut("{userId}/disable")]
        public async Task<IActionResult> DisableAsync(Guid userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return BadRequest(new {Code = 400, Msg = "User not exists"});
            }

            if (user.UserName == AdminConsts.AdminName)
            {
                return BadRequest(new {Code = 400, Msg = "Disable admin is not allowed"});
            }

            var result = await _userManager.SetLockoutEnabledAsync(user, true);
            if (result.Succeeded)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            }

            return Ok(new {Code = 200, Msg = "Disable success"});
        }

        [HttpPut("{userId}/enable")]
        public async Task<IActionResult> EnableAsync(Guid userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return BadRequest(new {Code = 400, Msg = "User not exists"});
            }

            var result = await _userManager.SetLockoutEnabledAsync(user, false);
            if (result.Succeeded)
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MinValue);
                await _userManager.ResetAccessFailedCountAsync(user);
            }

            return Ok(new {Code = 200, Msg = "Enable success"});
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteAsync(Guid userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return BadRequest(new {Code = 400, Msg = "User not exists"});
            }

            if (user.UserName == AdminConsts.AdminName)
            {
                return BadRequest(new {Code = 400, Msg = "Delete admin is not allowed"});
            }

            var result = await _userManager.DeleteAsync(user);
            return IdentityResult(result);
        }

        #endregion

        #region User Role

        [HttpPost("{userId}/role/{role}")]
        public async Task<IActionResult> CreateUserRoleAsync(Guid userId, string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                return BadRequest(new {Code = 400, Msg = "Role name should not be null/empty"});
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return BadRequest(new {Code = 400, Msg = "User not exists"});
            }

            if (user.UserName == AdminConsts.AdminName)
            {
                return BadRequest(new {Code = 400, Msg = "Add role to admin is not allowed"});
            }

            var result = await _userManager.AddToRoleAsync(user, role);
            return IdentityResult(result);
        }


        [HttpDelete("{userId}/role/{roleId}")]
        public async Task<IActionResult> DeleteUserRoleAsync(Guid userId, Guid roleId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return BadRequest(new {Code = 400, Msg = "User not exists"});
            }

            if (user.UserName == AdminConsts.AdminName)
            {
                return BadRequest(new {Code = 400, Msg = "Remove role from admin is not allowed"});
            }

            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                return BadRequest(new {Code = 400, Msg = "Role not exists"});
            }

            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);
            return IdentityResult(result);
        }

        #endregion
    }
}