using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace AppAdmin.Resources;

[ServiceStack.Route("[controller]/[action]")]
public class CultureController : Controller
{
    public IActionResult SetCulture(string? culture, string redirectUri)
    {
        if (culture != null)
        {
            HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture, culture)));
        }

        return LocalRedirect(redirectUri);
    }

    public IActionResult ResetCulture(string redirectUri)
    {
        HttpContext.Response.Cookies.Delete(CookieRequestCultureProvider.DefaultCookieName);

        return LocalRedirect(redirectUri);
    }
}