using Microsoft.AspNetCore.Identity;

namespace Hrnetgroup.Wms.Domain.ApplicationUsers;

public class AppUser : IdentityUser
{
    public DateTime? DateOfBirth { get; set; }
    
    public string Name { get; set; }
    
    public string Surname { get; set; }
    
    public string ExtensionData { get; set; }
}