using Microsoft.Extensions.Configuration;

namespace IdentityServer4.Admin.Infrastructure
{
    public class AdminOptions
    {
        private readonly IConfiguration _configuration;

        public AdminOptions(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Version => _configuration["Version"];

        public bool OpenRegister => _configuration["OpenRegister"].CastTo<bool>();

        public string SiteName => string.IsNullOrWhiteSpace(_configuration["SiteName"])
            ? "IdentityServer4 Admin"
            : _configuration["SiteName"];

        public string StorageRoot => _configuration["StorageRoot"];
        
        public bool RequireUppercase =>
            !string.IsNullOrWhiteSpace(_configuration["RequireUppercase"]) &&
            _configuration["RequireUppercase"].CastTo<bool>();

        public bool RequireNonAlphanumeric =>
            !string.IsNullOrWhiteSpace(_configuration["RequireNonAlphanumeric"]) &&
            _configuration["RequireNonAlphanumeric"].CastTo<bool>();

        public bool RequireDigit =>
            !string.IsNullOrWhiteSpace(_configuration["RequireDigit"]) &&
            _configuration["RequireDigit"].CastTo<bool>();

        public int RequiredLength => _configuration["RequiredLength"].CastTo<int>();

        public bool RequireUniqueEmail => !string.IsNullOrWhiteSpace(_configuration["RequireUniqueEmail"]) &&
                                          _configuration["RequireUniqueEmail"].CastTo<bool>();

        public bool EnableTokenCleanup => string.IsNullOrWhiteSpace(_configuration["EnableTokenCleanup"]) ||
                                          _configuration["EnableTokenCleanup"].CastTo<bool>();

        public bool AllowAnonymousUserQuery => string.IsNullOrWhiteSpace(_configuration["AllowAnonymousUserQuery"]) ||
                                               _configuration["AllowAnonymousUserQuery"].CastTo<bool>();

        public string ConnectionString => _configuration["ConnectionString"];

        public string DatabaseProvider => _configuration["DatabaseProvider"];

        public bool AllowLocalLogin => string.IsNullOrWhiteSpace(_configuration["AllowLocalLogin"]) ||
                                       _configuration["AllowLocalLogin"].CastTo<bool>();

        public bool AllowRememberLogin => string.IsNullOrWhiteSpace(_configuration["AllowRememberLogin"]) ||
                                          _configuration["AllowRememberLogin"].CastTo<bool>();

        public int RememberMeLoginDuration => string.IsNullOrWhiteSpace(_configuration["RememberMeLoginDuration"])
            ? 30
            : _configuration["RememberMeLoginDuration"].CastTo<int>();

        public bool ShowLogoutPrompt => string.IsNullOrWhiteSpace(_configuration["ShowLogoutPrompt"]) ||
                                        _configuration["ShowLogoutPrompt"].CastTo<bool>();

        public bool AutomaticRedirectAfterSignOut =>
            !string.IsNullOrWhiteSpace(_configuration["AutomaticRedirectAfterSignOut"]) &&
            _configuration["AutomaticRedirectAfterSignOut"].CastTo<bool>();

        // specify the Windows authentication scheme being used
        public string WindowsAuthenticationSchemeName => "Windows";

        // if user uses windows auth, should we load the groups from windows
        public bool IncludeWindowsGroups => !string.IsNullOrWhiteSpace(_configuration["IncludeWindowsGroups"]) &&
                                            _configuration["IncludeWindowsGroups"].CastTo<bool>();

        public string InvalidCredentialsErrorMessage { get; set; } = "用户名或密码错误";

        public bool EnableOfflineAccess => string.IsNullOrWhiteSpace(_configuration["EnableOfflineAccess"]) ||
                                           _configuration["EnableOfflineAccess"].CastTo<bool>();

        public string OfflineAccessDisplayName = "Offline Access";

        public string OfflineAccessDescription => string.IsNullOrWhiteSpace(_configuration["OfflineAccessDescription"])
            ? "Access to your applications and resources, even when you are offline"
            : _configuration["AllowRememberLogin"];


        public string MustChooseOneErrorMessage =>
            string.IsNullOrWhiteSpace(_configuration["MustChooseOneErrorMessage"])
                ? "You must pick at least one permission"
                : _configuration["MustChooseOneErrorMessage"];

        public string InvalidSelectionErrorMessage =>
            string.IsNullOrWhiteSpace(_configuration["MustChooseOneErrorMessage"])
                ? "Invalid selection"
                : _configuration["InvalidSelectionErrorMessage"];
    }
}