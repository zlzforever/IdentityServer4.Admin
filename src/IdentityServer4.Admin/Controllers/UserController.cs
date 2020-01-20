using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using X.PagedList;

namespace IdentityServer4.Admin.Controllers
{
    [Authorize]
    [Route("user")]
    public partial class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IDbContext _dbContext;
        private readonly IMapper _mapper;
        
        public UserController(UserManager<User> userManager,
            IDbContext dbContext,
            IMapper  mapper,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpGet]
        public async Task<IActionResult> Index(KeywordPagedQuery input)
        {
            var queryable = _userManager.Users;
            if (!string.IsNullOrWhiteSpace(input.Q))
            {
                queryable = queryable.Where(
                    u => $"{u.Email}".Contains(input.Q) ||
                         $"{u.UserName}".Contains(input.Q) ||
                         $"{u.PhoneNumber}".Contains(input.Q) ||
                         // Comment: 如果不拼接成字符串报空引用错, Lewis Zou, 2018-12-10
                         $"{u.FamilyName}{u.GivenName}".Contains(input.Q) ||
                         $"{u.GivenName}{u.FamilyName}".Contains(input.Q)
                );
            }

            var queryResult = await queryable.OrderBy(x => x.CreationTime).AsNoTracking()
                .ToPagedListAsync(input.GetPage(), input.GetSize());

            var userItemViewModels = new List<ListUserItemViewModel>();
            foreach (var user in queryResult)
            {
                var dto = _mapper.Map<ListUserItemViewModel>(user);
                dto.IsLockedOut = await _userManager.IsLockedOutAsync(user);
                //TODO: 需要优化成一次查询
                dto.Roles = string.Join("; ", await _userManager.GetRolesAsync(user));
                userItemViewModels.Add(dto);
            }

            var pagedList = new StaticPagedList<ListUserItemViewModel>(userItemViewModels, queryResult.PageNumber,
                queryResult.PageSize, queryResult.TotalItemCount);
            var viewModel = new ListUserViewModel {Users = pagedList, Keyword = input.Q};
            return View(viewModel);
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpGet("{userId}")]
        public async Task<IActionResult> ViewAsync(Guid userId, string returnUrl)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound();
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View("View", _mapper.Map<ViewUserViewModel>(user));
        }
    }
}