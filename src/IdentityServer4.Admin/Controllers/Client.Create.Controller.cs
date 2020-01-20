using System.Threading.Tasks;
using IdentityServer4.Admin.ViewModels.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer4.Admin.Controllers
{
    public partial class ClientController
    {
        [HttpGet("create")]
        public Task<IActionResult> CreateAsync(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["RenderSecrets"] = false;
            return Task.FromResult<IActionResult>(View("Create", new ClientViewModel()));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAsync(string returnUrl, ClientViewModel dto)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", dto);
            }

            if (await _dbContext.Clients.AnyAsync(u => u.ClientId == dto.ClientId))
            {
                ModelState.AddModelError("ClientId", "Client already exits");
                return View("Create", dto);
            }

            await _dbContext.Clients.AddAsync(dto.ToClient());
            await _dbContext.SaveChangesAsync();
            if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Index");
            }

            return Redirect(returnUrl);
        }
    }
}