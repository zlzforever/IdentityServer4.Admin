using X.PagedList;

namespace IdentityServer4.Admin.ViewModels.User
{
    public class ListUserViewModel
    {
        public IPagedList<ListUserItemViewModel> Users { get; set; }

        public string Keyword { get; set; }
    }
}