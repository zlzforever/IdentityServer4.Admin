using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace IdentityServer4.Admin.Infrastructure
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Sex
    {
        Male,
        Female
    }
}