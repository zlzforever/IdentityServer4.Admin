using X.PagedList;

namespace IdentityServer4.Admin.ViewModels.Role
{
    public class ListRoleViewModel
    {
        public IPagedList<ListRoleItemViewModel> Roles { get; set; }

        public string Keyword { get; set; }
    }
}