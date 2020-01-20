using System;

namespace IdentityServer4.Admin.Infrastructure
{
    public class LogHelper
    {
        public static string GetLogPathFormat(string app)
        {
            var date = $"logs/{DateTimeOffset.Now.ToLocalTime():yyyyMMdd}";
            return $"{date}/{app}.log";
        }
    }
}