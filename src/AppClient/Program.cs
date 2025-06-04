using System.Globalization;
using System.Net;
using AppClient.Components;
using AppClient.Components.Account;
using AppClient.Components.Account.Pages;
using AppClient.Data;
using AppClient.Resources;
using AppClient.State;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServiceStack.Blazor;
using Shared;
using Shared.Data.DbContext;
using Shared.GridLayouts.Services;
using Shared.Notifications;
using Shared.Organizations.Services;
using Shared.Users.Entities;
using Shared.Users.Services;
using Syncfusion.Blazor;
using Telerik.Blazor.Services;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");

if (!builder.Environment.IsEnvironment("Development"))
{
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo(@"/app/data"))
        .SetApplicationName("AppClient");
}

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("lic");
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddOutputCache();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();

builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<AppStateFactory>(); // Lägg till en factory-tjänst

// Använd factory-tjänsten för att hantera instanser
builder.Services.AddScoped(sp => sp.GetRequiredService<AppStateFactory>().CreateAppState(sp.GetRequiredService<UserService>(), sp.GetRequiredService<OrganizationService>()));
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.Services.AddTelerikBlazor();
builder.Services.AddSingleton<ITelerikStringLocalizer, LocalizationService>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    // The list of cultures that the app will support.
    var supportedCultures = new List<CultureInfo>()
    {
        new CultureInfo("sv-SE"),
        new CultureInfo("en-US"),
        new CultureInfo("da-DK"),
        new CultureInfo("de-DE"),
        new CultureInfo("es-ES"),
        new CultureInfo("fr-FR"),
        new CultureInfo("it-IT"),
        new CultureInfo("nb-NO")
    };

    // Set the default culture.
    options.DefaultRequestCulture = new RequestCulture("sv-SE");

    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddFacebook(facebookOptions =>
    {
        facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"]!;
        facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"]!;
    })
    .AddTwitter(twitterOptions =>
    {
        twitterOptions.ConsumerKey = builder.Configuration["Authentication:Twitter:APIKey"];
        twitterOptions.ConsumerSecret = builder.Configuration["Authentication:Twitter:APIKeySecret"];
    })
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    })
    .AddMicrosoftAccount(microsoftOptions =>
    {
        microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientID"]!;
        microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:SecretID"]!;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, optionsBuilder =>
            optionsBuilder.MigrationsAssembly("Server")
                .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging()
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, optionsBuilder =>
            optionsBuilder.MigrationsAssembly("Server")
                .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging()
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorizationCore(config =>
{
    config.AddPolicy("User", policy => policy.RequireClaim("userType", "User"));
    config.AddPolicy("Admin", policy => policy.RequireClaim("userType", "Admin"));
    config.AddPolicy("CEO", policy => policy.RequireClaim("role", "CEO"));
    config.AddPolicy("CFO", policy => policy.RequireClaim("role", "CFO"));
    config.AddPolicy("PG", policy => policy.RequireClaim("role", "PG"));
    config.AddPolicy("PC", policy => policy.RequireClaim("role", "PC"));
    config.AddPolicy("SFA", policy => policy.RequireClaim("role", "SFA"));
    config.AddPolicy("IFA", policy => policy.RequireClaim("role", "IFA"));
    config.AddPolicy("KFA", policy => policy.RequireClaim("role", "KFA"));
    config.AddPolicy("DFA", policy => policy.RequireClaim("role", "DFA"));
    config.AddPolicy("TVA", policy => policy.RequireClaim("role", "TVA"));
    config.AddPolicy("EA", policy => policy.RequireClaim("role", "EA"));
    config.AddPolicy("DA", policy => policy.RequireClaim("role", "DA"));
    config.AddPolicy("PRK", policy => policy.RequireClaim("role", "PRK"));
    config.AddPolicy("PLC", policy => policy.RequireClaim("role", "PLC"));
    config.AddPolicy("AA", policy => policy.RequireClaim("role", "AA"));
    config.AddPolicy("MK", policy => policy.RequireClaim("role", "MK"));
    config.AddPolicy("PK", policy => policy.RequireClaim("role", "PK"));
    config.AddPolicy("DK", policy => policy.RequireClaim("role", "DK"));
    config.AddPolicy("PR", policy => policy.RequireClaim("role", "PR"));
    config.AddPolicy("ADM", policy => policy.RequireClaim("role", "ADM"));
    config.AddPolicy("P", policy => policy.RequireClaim("role", "P"));
    config.AddPolicy("All", policy => policy.RequireClaim("role", "ALL"));
});

builder.Services.AddSingleton<IEmailSender<User>, IdentityNoOpEmailSender>();

builder.Services.RegisterFundIt(builder.Configuration);
builder.Services.AddLocalStorage();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<GridLayoutService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddSyncfusionBlazor();

var app = builder.Build();

app.UseFundIt();
var requestLocalizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>()?.Value;
if (requestLocalizationOptions != null) app.UseRequestLocalization(requestLocalizationOptions);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.MapAdditionalIdentityEndpoints();
app.MapUploadEndpoints();

app.MapDefaultEndpoints();
app.Run();
