using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Quartz;
using Shared.Data.DbContext;
using Shared.Global.Entities;
using Shared.OpenAi.Entities;
using SmartComponents.LocalEmbeddings;
using Telerik.Blazor.Components;

namespace Shared.Jobs.QuartzJobs;

public class QuartzOpenAiProjectsJob(IServiceScopeFactory serviceScopeFactory) : IJob
{
    public static readonly JobKey Key = new JobKey("QuartzOpenAiProjects", "SingleJob");

    public async Task Execute(IJobExecutionContext jobExecutionContext)
    {
        Console.WriteLine("<= QuartzOpenAiProjectsJob starting...");
        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            await using var context = await factory.CreateDbContextAsync(jobExecutionContext.CancellationToken);
            var embedder = scope.ServiceProvider.GetRequiredService<LocalEmbedder>();
            var projectService = scope.ServiceProvider.GetRequiredService<ProjectService>();
            var schedulerFactory = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();
            var scheduler = await schedulerFactory.GetScheduler(jobExecutionContext.CancellationToken);

            // Start timer
            var timer = Stopwatch.StartNew();
            Console.WriteLine($"<= QuartzOpenAiProjects started" + $", at: {DateTime.UtcNow:hh:mm:ss}");
            
            // Get all projects
            var projects = await context.Projects.Where(x => x.Id > 0).ToListAsync();
            
            // Loop through all projects
            foreach (var project in projects)
            {
                await Task.Delay(TimeSpan.FromSeconds(30));
                
                var jobKey = new JobKey($"QuartzOpenAiProjectJobOnce{project.Id}", "SingleJob");

                var openAiProjectJob = JobBuilder.Create<QuartzOpenAiProjectJob>()
                    .WithIdentity(jobKey)
                    .UsingJobData("projectId", project.Id)
                    .StoreDurably()
                    .Build();

                await scheduler.AddJob(openAiProjectJob, true);

                await scheduler.TriggerJob(jobKey);
            }
            
            Console.WriteLine($"<= QuartzOpenAiProjects succeeded" + $", total processing time: {timer.Elapsed:mm\\:ss}");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("<= QuartzOpenAiProjects failed " + ex);
        }
    }
    
}