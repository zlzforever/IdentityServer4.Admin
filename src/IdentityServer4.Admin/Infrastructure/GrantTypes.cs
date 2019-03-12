namespace IdentityServer4.Admin.Infrastructure
{
    public enum GrantTypes
    {
        Implicit,
        ImplicitAndClientCredentials,
        Code,
        CodeAndClientCredentials,
        Hybrid,
        HybridAndClientCredentials,
        ClientCredentials,
        ResourceOwnerPassword,
        ResourceOwnerPasswordAndClientCredentials,
        DeviceFlow
    }
}