namespace IdentityServer4.Admin.ViewModels.Home
{
    public class DashboardViewModel  
    {
        public int UserCount { get; set; }
        public int LockedUserCount { get; set; }
        public int ClientCount{ get; set; }
        public int ApiResourceCount { get; set; }
    }
}