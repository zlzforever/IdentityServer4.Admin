namespace IdentityServer4.Admin.ViewModels.ApiResource
{
    public class ListApiResourceScopeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        
        public bool Required { get; set; }
        
        public bool Emphasize { get; set; }
        
        public bool ShowInDiscoveryDocument { get; set; } = true;
        
        public string UserClaims { get; set; }
    }
    

}