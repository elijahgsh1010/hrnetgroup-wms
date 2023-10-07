using Hrnetgroup.Wms.Domain.ApplicationUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Hrnetgroup.Wms.Application;

[Authorize]
public class AccountAppService
{
    protected readonly UserManager<AppUser> UserManager;
    
    public AccountAppService(UserManager<AppUser> userManager)
    {
        UserManager = userManager;
    }

    [AllowAnonymous]
    public virtual async Task Register()
    {
        var user = new IdentityUser("");

        await UserManager.CreateAsync(new AppUser(), "123qwe");
    }
    
}