using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Users.Entities;

namespace Shared.Extensions;
public static class IdentityComponentsEndpointRouteBuilderExtensions
{
    // These endpoints are required by the Identity Razor components defined in the /Components/Pages/Account directory of this project.
    public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var accountGroup = endpoints.MapGroup("/Account");

        //accountGroup.MapPost("/PerformExternalLogin", (
        //    HttpContext context,
        //    [FromServices] SignInManager<User> signInManager,
        //    [FromForm] string provider,
        //    [FromForm] string returnUrl) =>
        //{
        //    IEnumerable<KeyValuePair<string, StringValues>> query = [
        //        new("ReturnUrl", returnUrl),
        //        new("Action", ExternalLogin.LoginCallbackAction)];

        //    var redirectUrl = UriHelper.BuildRelative(
        //        context.Request.PathBase,
        //        $"/Account/ExternalLogin",
        //        QueryString.Create(query));

        //    var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        //    return Results.Challenge(properties, [provider]);
        //});

        var manageGroup = accountGroup.MapGroup("/Manage").RequireAuthorization();

        //manageGroup.MapPost("/LinkExternalLogin", async (
        //    HttpContext context,
        //    [FromServices] SignInManager<User> signInManager,
        //    [FromForm] string provider) =>
        //{
        //    // Clear the existing external cookie to ensure a clean login process
        //    await context.SignOutAsync(IdentityConstants.ExternalScheme);

        //    var redirectUrl = UriHelper.BuildRelative(
        //        context.Request.PathBase,
        //        $"/Account/Manage/ExternalLogins",
        //        QueryString.Create("Action", ExternalLogins.LinkLoginCallbackAction));

        //    var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, signInManager.UserManager.GetUserId(context.User));
        //    return Results.Challenge(properties, [provider]);
        //});

        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var downloadLogger = loggerFactory.CreateLogger("DownloadPersonalData");

        manageGroup.MapPost("/DownloadPersonalData", async (
            HttpContext context,
            [FromServices] UserManager<User> userManager,
            [FromServices] AuthenticationStateProvider authenticationStateProvider) =>
        {
            var user = await userManager.GetUserAsync(context.User);
            if (user is null)
            {
                return Results.NotFound($"Unable to load user with ID '{userManager.GetUserId(context.User)}'.");
            }

            var userId = await userManager.GetUserIdAsync(user);
            downloadLogger.LogInformation("User with ID '{UserId}' asked for their personal data.", userId);

            // Only include personal data for download
            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(User).GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            var logins = await userManager.GetLoginsAsync(user);
            foreach (var l in logins)
            {
                personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }

            personalData.Add($"Authenticator Key", (await userManager.GetAuthenticatorKeyAsync(user))!);
            var fileBytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(personalData);

            context.Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
            return Results.File(fileBytes, contentType: "application/json", fileDownloadName: "PersonalData.json");
        });

        return accountGroup;
    }
}
