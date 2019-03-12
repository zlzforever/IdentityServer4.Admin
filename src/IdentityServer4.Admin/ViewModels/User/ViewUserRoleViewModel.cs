using System.Collections.Generic;
using IdentityServer4.Admin.ViewModels.Role;

namespace IdentityServer4.Admin.ViewModels.User
{
    public class ViewUserRoleViewModel
    {
        public List<ListRoleItemViewModel> UserRoles { get; set; }
        
        public List<ListRoleItemViewModel> AvailableRoles { get; set; }
    }
}