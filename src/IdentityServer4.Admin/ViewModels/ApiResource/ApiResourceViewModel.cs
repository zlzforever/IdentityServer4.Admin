using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Admin.ViewModels.ApiResource
{
    public class ApiResourceViewModel
    {
        /// <summary>
        /// Indicates if this resource is enabled. Defaults to true.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// The unique name of the resource.
        /// </summary>
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; }

        /// <summary>
        /// Display name of the resource.
        /// </summary>
        [StringLength(200)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Description of the resource.
        /// </summary>
        [StringLength(1000)]
        public string Description { get; set; }

        /// <summary>
        /// List of accociated user claims that should be included when this resource is requested.
        /// </summary>
        public string UserClaims { get; set; }
    }
}