
using System.Reflection;
using System.Text;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quartz;
using Shared.ApiKeys;
using Shared.AuditEntries.Entities;
using Shared.Controls.Services;
using Shared.Data.DbContext;
using Shared.Data.Interceptors;
using Shared.Email.Entities;
using Shared.Email.Services;
using Shared.Events.Services;
using Shared.Global.Interface;
using Shared.Global.Services;
using Shared.GoogleDrive.Services;
using Shared.GridLayouts.Services;
using Shared.HealthChecks;
using Shared.Jobs.QuartzJobs;
using Shared.MessageQueue.Services;
using Shared.OpenAi.Services;
using Shared.Schemas.Services;
using Shared.Statistics.Services;
using Shared.Users.Services;
using SmartComponents.LocalEmbeddings;

namespace Server;

public static class Configurations
{
    public static WebApplicationBuilder ConfigureHealthChecks(this WebApplicationBuilder builder)
    {
        if (!builder.Configuration.GetValue<bool>("UseHealthCheck")) return builder;
        
        builder.Services.AddHealthChecks()
            .AddSqlite(builder.Configuration["ConnectionStrings:DbContext"]!, 
                healthQuery: "select 1", 
                name: "Fund IT SQLite",
                failureStatus: HealthStatus.Unhealthy, 
                tags: new[] { "Feedback", "Database" })
            .AddSqlite(builder.Configuration["ConnectionStrings:Hangfire"]!, 
                healthQuery: "select 1", 
                name: "Hangfire SQLite",
                failureStatus: HealthStatus.Unhealthy, 
                tags: new[] { "Feedback", "Database" })
            .AddCheck<RemoteHealthCheckOpenExchangeRate>("Remote endpoints Health Check for OpenExchangeRate", 
                failureStatus: HealthStatus.Unhealthy)
            .AddCheck<MemoryHealthCheck>($"Feedback Service Memory Check", 
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "Feedback", "Service" });

        builder.Services.AddHealthChecksUI(opt =>
            {
                opt.SetEvaluationTimeInSeconds(60); //time in seconds between check    
                opt.MaximumHistoryEntriesPerEndpoint(120); //maximum history of checks    
                opt.SetApiMaxActiveRequests(1); //api requests concurrency    
                opt.AddHealthCheckEndpoint("feedback api", "/health"); //map health check api    
                opt.SetNotifyUnHealthyOneTimeUntilChange(); // You will only receive one failure notification until the status changes.
            })
            .AddSqliteStorage(builder.Configuration["ConnectionStrings:Healthcheck"]!);

        return builder;
    }

    public static WebApplicationBuilder RegisterEndpointsApiExplorer(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();

        return builder;
    }

    public static WebApplicationBuilder RegisterQuartz(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<QuartzOptions>(options =>
        {
            options.Scheduling.IgnoreDuplicates = true; // default: false
            options.Scheduling.OverWriteExistingData = true; // default: true
        });
        builder.Services.AddQuartz();
        builder.Services.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
            opt.AwaitApplicationStarted = true;
        });
        builder.Services.AddTransient<QuartzApplicationJob>();
        builder.Services.AddTransient<QuartzPrimaryApplicationsJob>();
        builder.Services.AddTransient<QuartzSecondaryApplicationsJob>();
        builder.Services.AddTransient<QuartzTertiaryApplicationsJob>();
        builder.Services.AddTransient<QuartzNotificationJob>();
        builder.Services.AddTransient<QuartzDocumentJob>();
        builder.Services.AddTransient<QuartzDocumentsJob>();
        builder.Services.AddTransient<QuartzOpenAiProjectJob>();
        builder.Services.AddTransient<QuartzStatisticsJob>();
        builder.Services.AddTransient<QuartzUsersJob>();
        builder.Services.AddTransient<QuartzOpenAiUsersJob>();
        builder.Services.AddTransient<QuartzMessagesJob>();
        builder.Services.AddTransient<QuartzMessageJob>();
        builder.Services.AddTransient<QuartzMilestonesJob>();
        builder.Services.AddTransient<QuartzOrganizationsJob>();
        builder.Services.AddTransient<QuartzProjectsJob>();
        builder.Services.AddTransient<QuartzTranslationsJob>();
        builder.Services.AddTransient<QuartzCleanUpJob>();
        builder.Services.AddTransient<QuartzDatabaseBackupJob>();
        builder.Services.AddTransient<QuartzProjectJob>();

        return builder;
    }
    
    public static WebApplicationBuilder RegisterAuthorization(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails();

        // For Identity
        builder.Services.AddAuthorization();
        builder.Services.AddIdentityCore<User>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
            options.User.RequireUniqueEmail = false;
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders().AddSignInManager();
        
        builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
            opt.TokenLifespan = TimeSpan.FromHours(2));

        // Adding Authentication
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

        // Adding Jwt Bearer
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]!))
                };
            });
        
        builder.Services.AddTransient<IEmailSender, EmailService>();

        return builder;
    }
    
    public static WebApplicationBuilder RegisterDatabase(this WebApplicationBuilder builder)
    {
        if (builder.Configuration.GetValue<bool>("UseAuditing"))
        {
            var auditEntries = new List<AuditEntry>();
            builder.Services.AddKeyedScoped<List<AuditEntry>>("Audit", (_, _) => auditEntries);
            builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DbContext")!, optionsBuilder => 
                        optionsBuilder.MigrationsAssembly("Server")
                            .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .AddInterceptors(new AuditInterceptor(auditEntries))
            );
            
            builder.Services.AddPooledDbContextFactory<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DbContext")!, optionsBuilder => 
                        optionsBuilder.MigrationsAssembly("Server")
                            .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .AddInterceptors(new AuditInterceptor(auditEntries))
            );
        }
        else
        {
            builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DbContext")!, optionsBuilder => 
                        optionsBuilder.MigrationsAssembly("Server")
                            .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))    
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            );
            
            builder.Services.AddPooledDbContextFactory<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DbContext")!, optionsBuilder => 
                        optionsBuilder.MigrationsAssembly("Server")
                            .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            );
        }
        
        return builder;
    }

    public static WebApplicationBuilder RegisterCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return builder;
    }
    
    public static WebApplicationBuilder RegisterHttpFactory(this WebApplicationBuilder builder)
    {

        builder.Services.AddHttpClient("api", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["API_BASE_ADDRESS"]!.ToString());
            })
            .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2)))
            .AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(5, TimeSpan.FromSeconds(10)));

        builder.Services.AddHttpClient("openai", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["OpenAi:Endpoint"]!.ToString());
            })
            .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2)))
            .AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(5, TimeSpan.FromSeconds(10)));

        return builder;
    }

    public static WebApplicationBuilder RegisterBackgroundQueue(this WebApplicationBuilder builder)
    {
        builder.Services.AddBackgroundTaskQueue();

        return builder;
    }

    public static WebApplicationBuilder RegisterEmailInformation(this WebApplicationBuilder builder)
    {
        var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
        builder.Services.AddSingleton(emailConfig!);
        builder.Services.AddScoped<EmailService>();

        return builder;
    }

    public static WebApplicationBuilder RegisterHostedBackgroundService(this WebApplicationBuilder builder)
    {
        builder.Services.AddHostedService<HostedService>();
        builder.Services.AddBackgroundTaskQueue();

        return builder;
    }

    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ApplicationService>();
        builder.Services.AddScoped<SchemaService>();
        builder.Services.AddScoped<ControlService>();
        builder.Services.AddScoped<EventService>();
        builder.Services.AddScoped<MessageService>();
        builder.Services.AddScoped<SharedService>();
        builder.Services.AddScoped<OrganizationService>();
        builder.Services.AddScoped<OpenAiService>();
        builder.Services.AddScoped<StatisticService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<AuthenticationService>();
        builder.Services.AddScoped<HtmlRenderer>();
        builder.Services.AddScoped<BlazorRenderer>();
        builder.Services.AddScoped<ProjectService>();
        builder.Services.AddScoped<GoogleDriveService>();
        builder.Services.AddScoped<MessageQueueService>();
        builder.Services.AddScoped<GridLayoutService>();
        
        builder.Services.AddSingleton<LocalEmbedder>();

        return builder;
    }
    
    public static WebApplicationBuilder RegisterEasyCache(this WebApplicationBuilder builder)
    {
        builder.Services.AddEasyCaching(options =>
        {
            options.UseInMemory(builder.Configuration, "default", "EasyCaching:InMemory");
        });

        return builder;
    }

    public static WebApplication ForceHttps(this WebApplication app, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("ForceHttps")) app.UseHsts();

        return app;
    }

    public static WebApplication MapHealthCheckAndUi(this WebApplication app, IConfiguration configuration)
    {
        if (!configuration.GetValue<bool>("UseHealthCheck")) return app;

        app.MapHealthChecks("/health", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        app.UseHealthChecksUI(options =>
        {
            options.UIPath = "/healthcheck-ui";
            options.AddCustomStylesheet("./HealthChecks/HealthCheck.css");

        });

        return app;
    }

    public static WebApplicationBuilder RegisterKeyValidator(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();

        return builder;
    }

    public static WebApplicationBuilder RegisterSwagger(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });

            if (configuration.GetValue<bool>("UseSwagger"))
            {
                option.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "X-API-KEY",
                    Type = SecuritySchemeType.ApiKey
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="ApiKey"
                            }
                        },
                        new string[]{}
                    }
                }); 
            }
        });

        return builder;
    }

    public static void RegisterEndpoints(this WebApplication app)
    {
        var assemblies = new List<Assembly>
        {
            typeof(Program).Assembly, // nuvarande assembly
            typeof(Shared.Schemas.Endpoints.SchemaEndpoints).Assembly // annan assembly, lägg till fler om det behövs
        };

        var endpointDefinitions = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(t => t.IsAssignableTo(typeof(IEndpoint)) && t is { IsAbstract: false, IsInterface: false })
            .Select(Activator.CreateInstance)
            .Cast<IEndpoint>();
        
        
        /*var endpointDefinitions = typeof(Program).Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IEndpoint)) && t is { IsAbstract: false, IsInterface: false })
            .Select(Activator.CreateInstance)
            .Cast<IEndpoint>();*/

        foreach (var endpointDef in endpointDefinitions)
        {
            endpointDef.RegisterEndpoints(app);
        }
    }

    public static IApplicationBuilder UseApiKeyValidation(this IApplicationBuilder app, IConfiguration configuration)
    {
        if (!configuration.GetValue<bool>("UseAPIKey")) return app;
        app.Use(async (context, next) =>
        {
            // If the request path contains /servernotifications, skip the API key validation
            if (context.Request.Path.Value is not null)
            {
                if (context.Request.Path.Value.Contains("hangfire") ||
                    context.Request.Path.Value.EndsWith("ping") ||
                    context.Request.Path.Value.EndsWith("health") ||
                    context.Request.Path.Value.Contains("healthchecks"))
                {
                    await next.Invoke();
                    return;
                }
            }
            // Get the API key from the X-API-KEY header
            var apiKey = context.Request.Headers["X-API-KEY"];

            // Validate the API key using the IApiKeyValidator service
            var apiKeyValidator = context.RequestServices.GetRequiredService<IApiKeyValidator>();
            if (!apiKeyValidator.Validate(apiKey))
            {
                // If the API key is invalid, return a 401 Unauthorized response
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            // If the API key is valid, pass the request to the next middleware in the pipeline
            await next.Invoke();
        });
        return app;
    }

    public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, IConfiguration configuration)
    {
        if (!configuration.GetValue<bool>("UseSwagger")) return app;
        app.UseSwagger();
        app.UseSwaggerUI();
        return app;
    }

}