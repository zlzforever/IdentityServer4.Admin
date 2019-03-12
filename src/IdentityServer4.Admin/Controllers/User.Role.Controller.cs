using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.ViewModels.Role;
using IdentityServer4.Admin.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace IdentityServer4.Admin.Controllers
{
    public partial class UserController
    {
        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpGet("{userId}/role")]
        public async Task<IActionResult> UserRoleAsync(Guid userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            var viewModel = new ViewUserRoleViewModel();
            var roles = _dbContext.UserRoles.Where(ur => ur.UserId == userId).Join(_dbContext.Roles, ur => ur.RoleId,
                    r => r.Id, (ur, r) =>
                        new ListRoleItemViewModel
                        {
                            Id = r.Id,
                            Name = r.Name,
                            Description = r.Description
                        })
                .ToList();
            viewModel.UserRoles = roles;
            var roleIds = await roles.Select(r => r.Id).ToListAsync();
            var availableRoles = await _dbContext.Roles.Where(x => !roleIds.Contains(x.Id)).Select(x =>
                new ListRoleItemViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                }).ToListAsync();
            viewModel.AvailableRoles = availableRoles;
            ViewData["UserId"] = userId;

            return View("Role", viewModel);
        }
    }
}