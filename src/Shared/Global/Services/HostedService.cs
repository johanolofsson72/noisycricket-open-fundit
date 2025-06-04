using Quartz;
using Shared.Data.DbContext;
using Shared.Jobs.QuartzJobs;

namespace Shared.Global.Services;
public class HostedService(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(0, stoppingToken);

        if (UnitTestDetector.IsInUnitTest) return;
        
        using var scope = serviceProvider.CreateScope();
        var schedulerFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();
        var scheduler = await schedulerFactory.GetScheduler(CancellationToken.None);
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        await using var context = await factory.CreateDbContextAsync(CancellationToken.None);
        
        // QuartzUsersJob
        var quartzUsersJob = JobBuilder.Create<QuartzUsersJob>()
            .WithIdentity($"QuartzUsersJob", "SingleJob")
            .Build();
        
        var quartzUsersTrigger = TriggerBuilder.Create()
            .WithIdentity($"QuartzUsersTrigger", "SingleTrigger")
            .WithSchedule(CronScheduleBuilder.CronSchedule(configuration.GetValue<string>("QuartzUsersTrigger")!)) // every 30 minutes between 6-21 on weekdays, starting at 6:02
            .Build();

        await scheduler.ScheduleJob(quartzUsersJob, quartzUsersTrigger, CancellationToken.None);
        
        // QuartzOpenAiUsersJob
        var quartzOpenAiUsersJob = JobBuilder.Create<QuartzOpenAiUsersJob>()
            .WithIdentity($"QuartzOpenAiUsersJob", "SingleJob")
            .Build();
        
        var quartzOpenAiUsersTrigger = TriggerBuilder.Create()
            .WithIdentity($"QuartzOpenAiUsersTrigger", "SingleTrigger")
            .WithSchedule(CronScheduleBuilder.CronSchedule(configuration.GetValue<string>("QuartzOpenAiUsersTrigger")!)) // every 30 minutes between 6-21 on weekdays, starting at 6:04
            .Build();

        await scheduler.ScheduleJob(quartzOpenAiUsersJob, quartzOpenAiUsersTrigger, CancellationToken.None);
        
        // QuartzOpenAiProjectsJob
        var QuartzOpenAiProjectsJob = JobBuilder.Create<QuartzOpenAiProjectsJob>()
            .WithIdentity($"QuartzOpenAiProjectsJob", "SingleJob")
            .Build();
        
        var QuartzOpenAiProjectsTrigger = TriggerBuilder.Create()
            .WithIdentity($"QuartzOpenAiProjectsTrigger", "SingleTrigger")
            .WithSchedule(CronScheduleBuilder.CronSchedule(configuration.GetValue<string>("QuartzOpenAiProjectsTrigger")!)) // every 30 minutes between 6-21 on weekdays, starting at 6:04
            .Build();

        await scheduler.ScheduleJob(QuartzOpenAiProjectsJob, QuartzOpenAiProjectsTrigger, CancellationToken.None);
        
        // QuartzMessagesJob
        var quartzMessagesJob = JobBuilder.Create<QuartzMessagesJob>()
            .WithIdentity($"QuartzMessagesJob", "SingleJob")
            .Build();
        
        var quartzMessagesTrigger = TriggerBuilder.Create()
            .WithIdentity($"QuartzMessagesTrigger", "SingleTrigger")
            .WithSchedule(CronScheduleBuilder.CronSchedule(configuration.GetValue<string>("QuartzMessagesTrigger")!)) // every 30 minutes between 6-21 on weekdays, starting at 6:06
            .Build();

        await scheduler.ScheduleJob(quartzMessagesJob, quartzMessagesTrigger, CancellationToken.None);
        
        // QuartzMilestonesJob
        var quartzMilestonesJob = JobBuilder.Create<QuartzMilestonesJob>()
            .WithIdentity($"QuartzMilestonesJob", "SingleJob")
            .Build();
        
        var quartzMilestonesTrigger = TriggerBuilder.Create()
            .WithIdentity($"QuartzMilestonesTrigger", "SingleTrigger")
            .WithSchedule(CronScheduleBuilder.CronSchedule(configuration.GetValue<string>("QuartzMilestonesTrigger")!)) // every 30 minutes between 6-21 on weekdays, starting at 6:08
            .Build();

        await scheduler.ScheduleJob(quartzMilestonesJob, quartzMilestonesTrigger, CancellationToken.None);
        
        // QuartzOrganizationsJob
        var quartzOrganizationsJob1 = JobBuilder.Create<QuartzOrganizationsJob>()
            .WithIdentity($"QuartzOrganizationsJob", "SingleJob")
            .Build();
        
        var quartzOrganizationsJob2 = JobBuilder.Create<QuartzOrganizationsJob>()
            .WithIdentity($"QuartzOrganizationsJob_Once", "SingleJob")
            .Build();
        
        var quartzOrganizationsTrigger1 = TriggerBuilder.Create()
            .WithIdentity($"QuartzOrganizationsTrigger", "SingleTrigger")
            .WithSchedule(CronScheduleBuilder.CronSchedule(configuration.GetValue<string>("QuartzOrganizationsTrigger")!)) // every 30 minutes between 6-21 on weekdays, starting at 6:10
            .Build();
        
        var quartzOrganizationsTrigger2 = TriggerBuilder.Create()
            .WithIdentity($"QuartzOrganizationsTriggerNow", "SingleTrigger")
            .StartNow()
            .Build();

        await scheduler.ScheduleJob(quartzOrganizationsJob1, quartzOrganizationsTrigger1, CancellationToken.None);
        await scheduler.ScheduleJob(quartzOrganizationsJob2, quartzOrganizationsTrigger2, CancellationToken.None);
        
        // QuartzDocumentsJob
        var quartzDocumentsJob = JobBuilder.Create<QuartzDocumentsJob>()
            .WithIdentity($"QuartzDocumentsJob", "SingleJob")
            .Build();
        
        var quartzDocumentsTrigger = TriggerBuilder.Create()
            .WithIdentity($"QuartzDocumentsTrigger", "SingleTrigger")
            .WithSchedule(CronScheduleBuilder.CronSchedule(configuration.GetValue<string>("QuartzDocumentsTrigger")!)) // every 30 minutes between 6-21 on weekdays, starting at 6:12
            .Build();

        await scheduler.ScheduleJob(quartzDocumentsJob, quartzDocumentsTrigger, CancellationToken.None);
        
        // QuartzPrimaryApplicationsJob
        var quartzPrimaryApplicationsJob1 = JobBuilder.Create<QuartzPrimaryApplicationsJob>()
            .WithIdentity($"QuartzPrimaryApplicationsJob", "SingleJob")
            .Build();
        
        var quartzPrimaryApplicationsJob2 = JobBuilder.Create<QuartzPrimaryApplicationsJob>()
            .WithIdentity($"QuartzPrimaryApplicationsJobOnce", "SingleJob")
            .Build();
        
        var quartzPrimaryApplicationsTrigger1 = TriggerBuilder.Create()
            .WithIdentity($"QuartzPrimaryApplicationsTrigger", "SingleTrigger")
            .WithSchedule(CronScheduleBuilder.CronSchedule(configuration.GetValue<string>("QuartzPrimaryApplicationsTrigger")!)) // every 30 minutes between 6-21 on weekdays, starting at 6:14
            .Build();
        
        var quartzPrimaryApplicationsTrigger2 = TriggerBuilder.Create()
            .WithIdentity($"QuartzPrimaryApplicationsTriggerOnce", "SingleTrigger")
            .StartNow()
            .Build();

        await scheduler.ScheduleJob(quartzPrimaryApplicationsJob1, quartzPrimaryApplicationsTrigger1, CancellationToken.None);
        await scheduler.ScheduleJob(quartzPrimaryApplicationsJob2, quartzPrimaryApplicationsTrigger2, new CancellationToken());
        
        // QuartzSecondaryApplicationsJob
        var quartzSecondaryApplicationsJob = JobBuilder.Create<QuartzSecondaryApplicationsJob>()
            .WithIdentity($"QuartzSecondaryApplicationsJob", "SingleJob")
            .Build();
        
        var quartzSecondaryApplicationsTrigger = TriggerBuilder.Create()
            .WithIdentity($"QuartzSecondaryApplicationsTrigger", "SingleTrigger")
            .WithSchedule(CronScheduleBuilder.CronSchedule(configuration.GetValue<string>("QuartzSecondaryApplicationsTrigger")!)) // every 30 minutes between 6-21 on weekdays, starting at 6:18
            .Build();

        await scheduler.ScheduleJob(quartzSecondaryApplicationsJob, quartzSecondaryApplicationsTrigger, CancellationToken.None);
        
        // QuartzTertiaryApplicationsJob
        var quartzTertiaryApplicationsJob = JobBuilder.Create<QuartzTertiaryApplicationsJob>()
            .WithIdentity($"QuartzTertiaryApplicationsJob", "SingleJob")
            .Build();
        
        var quartzTertiaryApplicationsTrigger = TriggerBuilder.Create()
            .WithIdentity($"QuartzTertiaryApplicationsTrigger", "SingleTrigger")
            .WithSchedule(CronScheduleBuilder.CronSchedule(configuration.GetValue<string>("QuartzTertiaryApplicationsTrigger")!)) // every 30 minutes between 6-21 on weekdays, starting at 6:22
            .Build();

        await scheduler.ScheduleJob(quartzTertiaryApplicationsJob, quartzTertiaryApplicationsTrigger, CancellationToken.None);
        
        // QuartzProjectsJob
        var quartzProjectsJob1 = JobBuilder.Create<QuartzProjectsJob>()
            .WithIdentity($"QuartzProjectsJob", "SingleJob")
            .Build();
        
        var quartzProjectsJob2 = JobBuilder.Create<QuartzProjectsJob>()
            .WithIdentity($"QuartzProjectsJobOnce", "SingleJob")
            .Build();
        
        var quartzProjectsTrigger1 = TriggerBuilder.Create()
            .WithIdentity($"QuartzProjectsTrigger", "SingleTrigger")
            .WithSchedule(CronScheduleBuilder.CronSchedule(configuration.GetValue<string>("QuartzProjectsTrigger")!)) // every 30 minutes between 6-21 on weekdays, starting at 6:26
            .Build();
        
        var quartzProjectsTrigger2 = TriggerBuilder.Create()
            .WithIdentity($"QuartzProjectsTriggerOnce", "SingleTrigger")
            .StartNow()
            .Build();

        await scheduler.ScheduleJob(quartzProjectsJob1, quartzProjectsTrigger1, CancellationToken.None);
        await scheduler.ScheduleJob(quartzProjectsJob2, quartzProjectsTrigger2, CancellationToken.None);
        
        // QuartzTranslationsJob
        var quartzTranslationsJob = JobBuilder.Create<QuartzTranslationsJob>()
            .WithIdentity($"QuartzTranslationsJob", "SingleJob")
            .Build();
        
        var quartzTranslationsTrigger = TriggerBuilder.Create()
            .WithIdentity($"QuartzTranslationsTrigger", "SingleTrigger")
            .WithSchedule(CronScheduleBuilder.CronSchedule(configuration.GetValue<string>("QuartzTranslationsTrigger")!)) // every day at 22:00
            .Build();

        await scheduler.ScheduleJob(quartzTranslationsJob, quartzTranslationsTrigger, CancellationToken.None);
        
        // QuartzCleanUpJob
        var quartzCleanUpJob = JobBuilder.Create<QuartzCleanUpJob>()
            .WithIdentity($"QuartzCleanUpJob", "SingleJob")
            .Build();
        
        var quartzCleanUpTrigger = TriggerBuilder.Create()
            .WithIdentity($"QuartzCleanUpTrigger", "SingleTrigger")
            .WithSchedule(CronScheduleBuilder.CronSchedule(configuration.GetValue<string>("QuartzCleanUpTrigger")!)) // every day at 23:00
            .Build();

        await scheduler.ScheduleJob(quartzCleanUpJob, quartzCleanUpTrigger, CancellationToken.None);
        
        // QuartzDatabaseBackupJob
        var quartzDatabaseBackupJob = JobBuilder.Create<QuartzDatabaseBackupJob>()
            .WithIdentity($"QuartzDatabaseBackupJob", "SingleJob")
            .Build();
        
        var quartzDatabaseBackupTrigger = TriggerBuilder.Create()
            .WithIdentity($"QuartzDatabaseBackupTrigger", "SingleTrigger")
            .WithSchedule(CronScheduleBuilder.CronSchedule(configuration.GetValue<string>("QuartzDatabaseBackupTrigger")!)) // every day at 23:30
            .Build();

        await scheduler.ScheduleJob(quartzDatabaseBackupJob, quartzDatabaseBackupTrigger, CancellationToken.None);
                
        // QuartzStatisticsJob
        var quartzStatisticsJob1 = JobBuilder.Create<QuartzStatisticsJob>()
            .WithIdentity($"QuartzStatisticsJob", "SingleJob")
            .Build();
        
        var quartzStatisticsJob2 = JobBuilder.Create<QuartzStatisticsJob>()
            .WithIdentity($"QuartzStatisticsJobOnce", "SingleJob")
            .Build();

        var quartzStatisticsTrigger1 = TriggerBuilder.Create()
            .WithIdentity($"QuartzStatisticsTrigger", "SingleTrigger")
            .WithSchedule(CronScheduleBuilder.CronSchedule(configuration.GetValue<string>("QuartzStatisticsTrigger")!)) // every 30 minutes between 6-21 on weekdays, starting at 6:00
            .Build();

        var quartzStatisticsTrigger2 = TriggerBuilder.Create()
            .WithIdentity($"QuartzStatisticsTriggerOnce", "SingleTrigger")
            .StartNow()
            .Build();

        await scheduler.ScheduleJob(quartzStatisticsJob1, quartzStatisticsTrigger1, CancellationToken.None);
        await scheduler.ScheduleJob(quartzStatisticsJob2, quartzStatisticsTrigger2, CancellationToken.None);
        
        /*var applications = await context.Applications.AsTracking().ToListAsync(CancellationToken.None);
        foreach (var application in applications)
        {
            var productionYear = application.Controls.FirstOrDefault(x => x.UniqueId.ToString().ToLower().StartsWith("978ac998"));

            if (productionYear is not null && productionYear.Value == "")
            {
                productionYear.Value = application.DeliveryDate.Year.ToString();
            } 
        }
        await context.SaveChangesAsync(CancellationToken.None);*/
        
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        // The code in here will run when the application stops
        Console.WriteLine(@"HostedService Killed...");
        return Task.CompletedTask;
    }
}

public static class UnitTestDetector
{
    static UnitTestDetector()
    {
        string testAssemblyName = "xunit";
        UnitTestDetector.IsInUnitTest = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName != null && a.FullName.StartsWith(testAssemblyName));
    }

    public static bool IsInUnitTest { get; private set; }
}