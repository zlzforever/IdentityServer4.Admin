using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.ViewModels.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using X.PagedList;

namespace IdentityServer4.Admin.Controllers
{
    [Authorize(Roles = AdminConsts.AdminName)]
    [Route("role")]
    public partial class RoleController : ControllerBase
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly IDbContext _dbContext;
        private readonly IMapper _mapper;
        
        public RoleController(RoleManager<Role> roleManager,
            IDbContext dbContext,
            IMapper  mapper,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _roleManager = roleManager;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(KeywordPagedQuery input)
        {
            var queryable = _dbContext.Roles.AsQueryable();
            if (!string.IsNullOrWhiteSpace(input.Q))
            {
                queryable = queryable.Where(
                    u => $"{u.Name}".Contains(input.Q) ||
                         $"{u.Description}".Contains(input.Q));
            }

            var queryResult = await queryable.OrderBy(x => x.CreationTime).AsNoTracking()
                .ToPagedListAsync(input.GetPage(), input.GetSize());
            var itemViewModels = new List<ListRoleItemViewModel>();
            foreach (var role in queryResult)
            {
                var dto = _mapper.Map<ListRoleItemViewModel>(role);
                itemViewModels.Add(dto);
            }

            var pagedList = new StaticPagedList<ListRoleItemViewModel>(itemViewModels, queryResult.PageNumber,
                queryResult.PageSize, queryResult.TotalItemCount);

            var viewModel = new ListRoleViewModel {Roles = pagedList, Keyword = input.Q};
            return View(viewModel);
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> ViewAsync(Guid roleId, string returnUrl)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                return NotFound();
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View("View", _mapper.Map<RoleViewModel>(role));
        }
    }
}