using System.Diagnostics;
using Quartz;
using Shared.MessageQueue.Services;
using Shared.Notifications;

namespace Shared.Jobs.QuartzJobs;

public class QuartzNotificationJob(IServiceScopeFactory serviceScopeFactory) : IJob
{
    public static readonly JobKey Key = new JobKey("QuartzNotification", "SingleJob");
    
    public async Task Execute(IJobExecutionContext jobExecutionContext)
    {
        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var cache = scope.ServiceProvider.GetRequiredService<IEasyCachingProvider>();
            var messageQueueService = scope.ServiceProvider.GetRequiredService<MessageQueueService>();
            var data = jobExecutionContext.JobDetail.JobDataMap;
            var projectId = data.GetInt("projectId");
            var applicationId = data.GetInt("applicationId");
            var notificationType = (NotificationType)data.GetInt("notificationType");
            
            // Start timer
            var timer = Stopwatch.StartNew();
            Console.WriteLine($"<= QuartzNotification started" + $", at: {DateTime.UtcNow:hh:mm:ss}");
            Console.WriteLine($"[QuartzNotification] ProjectId: {projectId}");
            
            switch (notificationType)
            {
                case NotificationType.Projects:
                    break;
                case NotificationType.Applications:
                    break;
                case NotificationType.ProjectsHeader:
                    await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), new CancellationToken());
                    _ = await messageQueueService.PublishAsync(NotificationType.ProjectsHeader, new CancellationToken());
                    break;
                case NotificationType.ApplicationOverview:
                    await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), new CancellationToken()); 
                    _ = await messageQueueService.PublishAsync(NotificationType.ApplicationOverview, new CancellationToken());
                    break;
                case NotificationType.Messages:
                    _ = await messageQueueService.PublishAsync(NotificationType.Messages, new CancellationToken());
                    break;
                case NotificationType.Economy:
                    await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), new CancellationToken()); 
                    _ = await messageQueueService.PublishAsync(NotificationType.Economy, new CancellationToken());
                    break;
                case NotificationType.Default:
                default:
                    break;
            }
            
            Console.WriteLine($"<= QuartzNotification succeeded" + $", total processing time: {timer.Elapsed:mm\\:ss}");

        }
        catch (Exception ex)
        {
            Console.WriteLine("<= QuartzNotification failed " + ex);
        }
    }
}