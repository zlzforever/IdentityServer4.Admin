using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.ViewModels.IdentityResource;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using X.PagedList;

namespace IdentityServer4.Admin.Controllers
{
    [Route("identity-resource")]
    public partial class IdentityResourceController : ControllerBase
    {
        private readonly IDbContext _dbContext;

        public IdentityResourceController(
            IDbContext dbContext,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _dbContext = dbContext;
        }

        [Authorize(Roles = AdminConsts.AdminName)]
        [HttpGet]
        public async Task<IActionResult> Index(PagedQuery input)
        {
            var queryable = _dbContext.IdentityResources.AsQueryable().OrderBy(x => x.Created);
            var queryResult = await queryable.ToPagedListAsync(input.GetPage(), input.GetSize());
            var ids = await queryResult.Select(x => x.Id).ToListAsync();
            var claims = _dbContext.IdentityClaims.Where(x => ids.Contains(x.IdentityResourceId))
                .ToList().GroupBy(x => x.IdentityResourceId).ToDictionary(g => g.Key);
            var viewModel = new StaticPagedList<ListIdentityResourceItemViewModel>(queryResult.Select(x =>
                    new ListIdentityResourceItemViewModel
                    {
                        Name = x.Name,
                        DisplayName = x.DisplayName,
                        Description = x.Description,
                        Emphasize = x.Emphasize,
                        Enabled = x.Enabled,
                        Required = x.Required,
                        Id = x.Id,
                        ShowInDiscoveryDocument = x.ShowInDiscoveryDocument,
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
            var identityResource = await _dbContext.IdentityResources.FirstOrDefaultAsync(x => x.Id == id);
            if (identityResource == null)
            {
                return NotFound();
            }

            var claims = await _dbContext.IdentityClaims.Where(x => x.IdentityResourceId == identityResource.Id)
                .Select(x => x.Type)
                .ToListAsync();
            var viewModel = new IdentityResourceViewModel
            {
                Name = identityResource.Name,
                Enabled = identityResource.Enabled,
                Required = identityResource.Required,
                Description = identityResource.Description,
                DisplayName = identityResource.DisplayName,
                ShowInDiscoveryDocument = identityResource.ShowInDiscoveryDocument,
                Emphasize = identityResource.Emphasize,
                UserClaims = string.Join(" ", claims)
            };
            ViewData["ReturnUrl"] = returnUrl;
            return View("View", viewModel);
        }
    }
}