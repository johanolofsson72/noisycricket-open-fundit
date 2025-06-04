using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Shared.Users.Entities;

namespace AppClient.Data;

public class VatSignInManager : SignInManager<User>
{
    public VatSignInManager(
        UserManager<User> userManager, 
        IHttpContextAccessor contextAccessor, 
        IUserClaimsPrincipalFactory<User> claimsFactory, 
        IOptions<IdentityOptions> optionsAccessor, 
        ILogger<SignInManager<User>> logger, 
        IAuthenticationSchemeProvider schemes, 
        IUserConfirmation<User> confirmation)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
    }

    public async Task<SignInResult> VatSignInAsync(string vatNumber, string email, string password, bool rememberMe, bool lockoutOnFailure)
    {
        // Anpassa din inloggningslogik här
        var user = await UserManager.FindByEmailAsync(email);
            
        if (user is null) return SignInResult.Failed;

        var exist = user.Organizations.Any(organization => organization.OrganizationVat.ToLower() == vatNumber);
            
        if (!exist) return SignInResult.Failed;

        return await PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure);
    }
}