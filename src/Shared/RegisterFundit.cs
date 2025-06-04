using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity.UI.Services;
using Polly;
using Quartz;
using Shared.Controls.Services;
using Shared.Documents.Services;
using Shared.Email.Entities;
using Shared.Email.Services;
using Shared.Events.Services;
using Shared.Global.Services;
using Shared.GoogleDrive.Services;
using Shared.Jobs.QuartzJobs;
using Shared.MessageQueue.Services;
using Shared.Milestones.Services;
using Shared.Notifications;
using Shared.OpenAi.Services;
using Shared.Schemas.Services;
using Shared.Statistics.Services;
using Shared.Users.Services;
using SmartComponents.LocalEmbeddings;

namespace Shared;

public static class RegisterFunditExtension
{
    public static IServiceCollection RegisterFundIt(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("api", client =>
            {
                client.BaseAddress = new Uri(configuration.GetValue<string>("API_BASE_ADDRESS")!);
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                };
            })
            .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2)))
            .AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(5, TimeSpan.FromSeconds(10)));
        

        services.AddScoped<ApplicationService>();
        services.AddScoped<SchemaService>();
        services.AddScoped<ControlService>();
        services.AddScoped<EventService>();
        services.AddScoped<MessageService>();
        services.AddScoped<SharedService>();
        services.AddScoped<EventService>();
        services.AddScoped<OrganizationService>();
        services.AddScoped<OpenAiService>();
        services.AddScoped<StatisticService>();
        services.AddScoped<UserService>();
        services.AddSingleton<LocalEmbedder>();
        services.AddSingleton<BrowserService>();
        services.AddScoped<HtmlRenderer>();
        services.AddScoped<BlazorRenderer>();
        services.AddScoped<DocumentService>();
        services.AddScoped<ProjectService>();
        services.AddScoped<MilestoneService>();
        services.AddScoped<NotificationService>();
        services.AddScoped<GoogleDriveService>();
        services.AddScoped<MessageQueueService>();
        
        services.Configure<QuartzOptions>(options =>
        {
            options.Scheduling.IgnoreDuplicates = true; // default: false
            options.Scheduling.OverWriteExistingData = true; // default: true
        });
        services.AddQuartz();
        services.AddQuartzHostedService(opt =>
        {
            opt.WaitForJobsToComplete = true;
            opt.AwaitApplicationStarted = true;
        });
        services.AddTransient<QuartzApplicationJob>();
        services.AddTransient<QuartzPrimaryApplicationsJob>();
        services.AddTransient<QuartzSecondaryApplicationsJob>();
        services.AddTransient<QuartzTertiaryApplicationsJob>();
        services.AddTransient<QuartzNotificationJob>();
        services.AddTransient<QuartzDocumentJob>();
        services.AddTransient<QuartzDocumentsJob>();
        services.AddTransient<QuartzOpenAiProjectJob>();
        services.AddTransient<QuartzStatisticsJob>();
        services.AddTransient<QuartzUsersJob>();
        services.AddTransient<QuartzOpenAiUsersJob>();
        services.AddTransient<QuartzMessagesJob>();
        services.AddTransient<QuartzMessageJob>();
        services.AddTransient<QuartzMilestonesJob>();
        services.AddTransient<QuartzOrganizationsJob>();
        services.AddTransient<QuartzProjectsJob>();
        services.AddTransient<QuartzTranslationsJob>();
        services.AddTransient<QuartzCleanUpJob>();
        services.AddTransient<QuartzDatabaseBackupJob>();
        services.AddTransient<QuartzProjectJob>();
        

// Suppress the diagnostic warning for the AddHybridCache method
#pragma warning disable EXTEXP0018
        services.AddHybridCache();
#pragma warning restore EXTEXP0018
        
        services.AddEasyCaching(options =>
        {
            options.UseInMemory(configuration, "default", "EasyCaching:InMemory");
        });
        
        services.AddTransient<IEmailSender, EmailService>();
        
        var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
        services.AddSingleton(emailConfig!);
        services.AddScoped<EmailService>();
        
        services.AddHttpClient("openai", client =>
            {
                client.BaseAddress = new Uri(configuration["OpenAi:Endpoint"]!.ToString());
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                };
            })
            .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2)))
            .AddTransientHttpErrorPolicy(policy => policy.CircuitBreakerAsync(5, TimeSpan.FromSeconds(10)));

        
        // register upload endpoints

        return services;
    }
    
    public static WebApplication UseFundIt(this WebApplication app)
    {

        return app;
    }
}