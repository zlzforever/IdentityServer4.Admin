using AutoMapper;
using IdentityServer4.Admin.ViewModels.Account;
using IdentityServer4.Admin.ViewModels.Role;
using IdentityServer4.Admin.ViewModels.User;

namespace IdentityServer4.Admin.ViewModels
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateUserViewModel, Entities.User>();
            CreateMap<Entities.Role, RoleViewModel>();
            CreateMap<Entities.Role, ViewUserRoleViewModel>();
            CreateMap<Entities.Role, ListRoleItemViewModel>();
            CreateMap<RoleViewModel, Entities.Role>();
            CreateMap<UpdateProfileViewModel, Entities.User>();
            CreateMap<UpdateProfileViewModel, ProfileViewModel>();
            CreateMap<Entities.User, ListUserItemViewModel>();
            CreateMap<ViewUserViewModel, Entities.User>();
        }
    }
}