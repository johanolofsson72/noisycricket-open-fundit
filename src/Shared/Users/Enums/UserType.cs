
using System.ComponentModel.DataAnnotations;

namespace Shared.Users.Enums
{
    public enum UserType
    {
        [Display(Name = "Default")]
        Default = 0,
    
        [Display(Name = "Client")]
        Client = 1,
    
        [Display(Name = "Admin")]
        Admin = 2
    }
}
