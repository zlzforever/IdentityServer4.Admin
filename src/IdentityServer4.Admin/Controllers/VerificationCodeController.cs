using IdentityServer4.Admin.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Admin.Controllers
{
    public class VerificationCodeController : ControllerBase
    {
        /// <summary>
        /// 图形验证码
        /// </summary>
        /// <returns></returns>
        public IActionResult New()
        {
            var bytes = VerificationCodeHelper.Create(out var code, 5);
            HttpContext.Session.SetString(AdminConsts.VerificationCode, code);
            Response.Body.Dispose();
            return File(bytes, @"image/png");
        }

        public VerificationCodeController(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }
    }
}