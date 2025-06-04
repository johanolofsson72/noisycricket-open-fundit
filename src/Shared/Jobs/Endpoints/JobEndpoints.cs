using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Quartz;
using Shared.Data.DbContext;
using Shared.Global.Interface;
using Shared.Jobs.QuartzJobs;
using Shared.Notifications;

namespace Shared.Jobs.Endpoints;

public class JobEndpoints : IEndpoint
{
    public void RegisterEndpoints(WebApplication app)
    {
        var group = app.MapGroup("/api/v1/jobs").WithName("Jobs").WithTags("Jobs");
        
        /*// This endpoint is not implemented
        group.MapPost("/refresh",
            async Task<Results<Ok, NotFound>> (JobService jobService, CancellationToken ct) =>
            {
                await Task.Delay(0, ct);
                
                //await jobService.RefreshAllJobs(ct);

                return TypedResults.Ok();
            })
        .WithName("Refresh (not implemented...)")
        .WithOpenApi();*/

        // This endpoint is done!
        group.MapPost("/aggregate/openaiprojects", 
                async Task<Results<Ok, NotFound>> (ISchedulerFactory schedulerFactory, CancellationToken ct) =>
                {
                    Console.WriteLine($"<= Aggregate OpenAiProjects");
                
                    var scheduler = await schedulerFactory.GetScheduler(ct);
                    
                    var openAiProjectJob = JobBuilder.Create<QuartzOpenAiProjectsJob>()
                        .WithIdentity($"QuartzOpenAiProjectsJobOnce", "SingleJob")
                        .Build();

                    var openAiProjectTrigger = TriggerBuilder.Create()
                        .WithIdentity($"QuartzOpenAiProjectsTrigger", "SingleTrigger")
                        .StartNow()
                        .Build();

                    await scheduler.ScheduleJob(openAiProjectJob, openAiProjectTrigger, CancellationToken.None);
                
                    return TypedResults.Ok();
                })
            .WithName("AggregateOpenAiProjects")
            .WithOpenApi();
        
        group.MapPost("/aggregate/applications/{applicationId}/{notificationType}", 
            async Task<Results<Ok, NotFound>> (ISchedulerFactory schedulerFactory, int applicationId, NotificationType notificationType, CancellationToken ct) =>
            {
                try
                {
                    Console.WriteLine($"<= Aggregate Applications: {applicationId}, {notificationType}");
                
                    var scheduler = await schedulerFactory.GetScheduler(ct);
                
                    var job = JobBuilder.Create<QuartzApplicationJob>()
                        .WithIdentity($"QuartzApplicationJob{applicationId}", "SingleJob")
                        .SetJobData(new JobDataMap()
                        {
                            ["applicationId"] = applicationId,
                            ["notificationType"] = (int)notificationType
                        })
                        .Build();

                    var trigger = TriggerBuilder.Create()
                        .WithIdentity($"QuartzApplicationTrigger{applicationId}", "SingleTrigger")
                        .StartNow()
                        .Build();

                    await scheduler.ScheduleJob(job, trigger, ct);
                
                    return TypedResults.Ok();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return TypedResults.NotFound();
                }
            })
        .WithName("AggregateApplications")
        .WithOpenApi();

        // This endpoint is done!
        group.MapPost("/aggregate/documents/{documentId}", 
            async Task<Results<Ok, NotFound>> (ISchedulerFactory schedulerFactory, int documentId, CancellationToken ct) =>
            {
                var scheduler = await schedulerFactory.GetScheduler(ct);
                
                var job = JobBuilder.Create<QuartzDocumentJob>()
                    .WithIdentity($"QuartzDocumentJob{documentId}", "SingleJob")
                    .SetJobData(new JobDataMap()
                    {
                        ["documentId"] = documentId
                    })
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity($"QuartzDocumentTrigger{documentId}", "SingleTrigger")
                    .StartNow()
                    .Build();

                await scheduler.ScheduleJob(job, trigger, ct);

                return TypedResults.Ok();
            })
        .WithName("AggregateDocuments")
        .WithOpenApi();

        // This endpoint is done!
        group.MapPost("/aggregate/messages/{messageId}", 
                async Task<Results<Ok, NotFound>> (ISchedulerFactory schedulerFactory, int messageId, CancellationToken ct) =>
                {
                    var scheduler = await schedulerFactory.GetScheduler(ct);
                
                    var job = JobBuilder.Create<QuartzMessageJob>()
                        .WithIdentity($"QuartzMessageJob{messageId}", "SingleJob")
                        .SetJobData(new JobDataMap()
                        {
                            ["messageId"] = messageId
                        })
                        .Build();

                    var trigger = TriggerBuilder.Create()
                        .WithIdentity($"QuartzMessageTrigger{messageId}", "SingleTrigger")
                        .StartNow()
                        .Build();

                    await scheduler.ScheduleJob(job, trigger, ct);

                    return TypedResults.Ok();
                })
            .WithName("AggregateMessages")
            .WithOpenApi();

        /*group.MapPost("/execute/event/{applicationId}", 
                async Task<Results<Ok, NotFound>> (IBackgroundTaskQueue backgroundQueue, ExecuteNextEventJob executeNextEventJob, CancellationToken ct, int applicationId) =>
                {
                    await Task.Delay(0, ct);
                    /*backgroundQueue.Enqueue(async _ =>
                    {
                        await executeNextEventJob.Start(applicationId);
                    });#1# 

                    return TypedResults.Ok();
                })
            .WithName("ExecuteNextEvent")
            .WithOpenApi();*/

        /*group.MapPost("/aggregate/organizations/{organizationId}", 
            async Task<Results<Ok, NotFound>> (IBackgroundTaskQueue backgroundQueue, AggregateOrganizationsJob aggregateOrganizationsJob, CancellationToken ct, int organizationId) =>
            {
                await Task.Delay(0, ct);
                /*backgroundQueue.Enqueue(async _ =>
                {
                    await aggregateOrganizationsJob.Start(organizationId);
                }); #1#

                return TypedResults.Ok();
            })
        .WithName("AggregateOrganizations")
        .WithOpenApi();*/

        group.MapPost("/aggregate/projects/{projectId}", 
            async Task<Results<Ok, NotFound>> (ISchedulerFactory schedulerFactory, int projectId, CancellationToken ct) =>
            {
                var scheduler = await schedulerFactory.GetScheduler(ct);
                
                var job = JobBuilder.Create<QuartzProjectJob>()
                    .WithIdentity($"QuartzProjectJob{projectId}", "SingleJob")
                    .SetJobData(new JobDataMap()
                    {
                        ["projectId"] = projectId
                    })
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity($"QuartzProjectTrigger{projectId}", "SingleTrigger")
                    .StartNow()
                    .Build();

                await scheduler.ScheduleJob(job, trigger, ct);

                return TypedResults.Ok();
            })
        .WithName("AggregateProjects")
        .WithOpenApi();

        /*group.MapPost("/aggregate/users/{userId}", 
            async Task<Results<Ok, NotFound>> (IBackgroundTaskQueue backgroundQueue, AggregateUsersJob aggregateUsersJob, CancellationToken ct, int userId) =>
            {
                await Task.Delay(0, ct);
                /*backgroundQueue.Enqueue(async _ =>
                {
                    await aggregateUsersJob.Start(userId);
                });#1# 

                return TypedResults.Ok();
            })
        .WithName("AggregateUsers")
        .WithOpenApi();*/

    }

}