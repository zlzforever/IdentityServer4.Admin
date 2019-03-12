using System;

namespace IdentityServer4.Admin.ViewModels.User
{
    public class ListUserItemViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Roles { get; set; }
        
        public bool IsLockedOut { get; set; }
        
        public Guid Id { get; set; }
    }
}