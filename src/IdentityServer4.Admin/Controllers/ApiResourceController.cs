using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.ViewModels.ApiResource;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using X.PagedList;

namespace IdentityServer4.Admin.Controllers
{
    [Authorize(Roles = AdminConsts.AdminName)]
    [Route("api-resource")]
    public partial class ApiResourceController : ControllerBase
    {
        private readonly IDbContext _dbContext;

        public ApiResourceController(
            IDbContext dbContext,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _dbContext = dbContext;
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpGet]
        public async Task<IActionResult> Index(PagedQuery input)
        {
            var queryable = _dbContext.ApiResources.AsQueryable().OrderBy(x => x.Created);
            var queryResult = await queryable.ToPagedListAsync(input.GetPage(), input.GetSize());
            var ids = await queryResult.Select(x => x.Id).ToListAsync();
            var claims = _dbContext.ApiResourceClaims.Where(x => ids.Contains(x.ApiResourceId)).ToList()
                .GroupBy(x => x.ApiResourceId).ToDictionary(g => g.Key);
            var viewModel = new StaticPagedList<ListApiResourceItemViewModel>(queryResult.Select(x =>
                    new ListApiResourceItemViewModel
                    {
                        Name = x.Name,
                        DisplayName = x.DisplayName,
                        Description = x.Description,
                        Enabled = x.Enabled,
                        Id = x.Id,
                        UserClaims = !claims.ContainsKey(x.Id)
                            ? ""
                            : string.Join(" ", claims[x.Id].Select(ic => ic.Type))
                    }), queryResult.PageNumber,
                queryResult.PageSize, queryResult.TotalItemCount);
            return View(viewModel);
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpGet("{id}")]
        public async Task<IActionResult> ViewAsync(int id, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var resource = await _dbContext.ApiResources.FirstOrDefaultAsync(x => x.Id == id);
            if (resource == null)
            {
                return NotFound();
            }

            var claims = await _dbContext.ApiResourceClaims.Where(x => x.ApiResourceId == resource.Id)
                .Select(x => x.Type)
                .ToListAsync();
            var viewModel = new ApiResourceViewModel
            {
                Name = resource.Name,
                Enabled = resource.Enabled,

                Description = resource.Description,
                DisplayName = resource.DisplayName,
                UserClaims = string.Join(" ", claims)
            };

            return View("View", viewModel);
        }
    }
}