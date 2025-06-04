using System.Diagnostics;
using System.Net;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Shared.Data.DbContext;
using Shared.Statistics.Services;

namespace Shared.Jobs.QuartzJobs;

public class QuartzMessagesJob(IServiceScopeFactory serviceScopeFactory) : IJob
{
    public static readonly JobKey Key = new JobKey("QuartzMessages", "SingleJob");
    
    public async Task Execute(IJobExecutionContext jobExecutionContext)
    {
        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            await using var context = await factory.CreateDbContextAsync(jobExecutionContext.CancellationToken);
            var cache = scope.ServiceProvider.GetRequiredService<IEasyCachingProvider>();

            // Start timer
            var timer = Stopwatch.StartNew();
            Console.WriteLine($"<= QuartzMessages started" + $", at: {DateTime.UtcNow:hh:mm:ss}");

            // Get messages
            var messages = await context.Messages
                .AsTracking()
                .Where(x => new List<int>(){16, 19, 23}.Contains(x.StatusId) == false)
                .ToListAsync() ?? throw new Exception("Can't find messages");

            foreach (var message in messages)
            {
                if (message.Receiver.ContactIdentifier != 0)
                {
                    message.Receiver.Name = GetMessageReceiverName(context, message.Receiver.ContactIdentifier);
                }

                if (message.ApplicationId != 0)
                {
                    var application = GetApplication(context, message.ApplicationId);

                    if (application is not null)
                    {
                        message.SchemaNames = application.SchemaNames;
                        message.ApplicationTitle = WebUtility.HtmlDecode(application.Title);
                        message.ApplicationStatusId = application.StatusId;
                    }
                }

                if (message.ProjectId == 0) continue;
                
                var project = GetProject(context, message.ProjectId);
                    
                if (project is null) continue;

                message.ProjectNumber = project.Number;
                message.ProjectTitle = project.Title.Count > 0 ? WebUtility.HtmlDecode(project.Title.First()) : "";
                
                var organization = GetOrganization(context, message.OrganizationId);
                
                message.OrganizationName = organization;
            }

            context.Messages.UpdateRange(messages);
            await context.SaveChangesAsync();
            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Messages.ToDescriptionString());
            
            Console.WriteLine($"<= QuartzMessages succeeded" + $", total processing time: {timer.Elapsed:mm\\:ss}");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("<= QuartzMessages failed " + ex);
        }
    }
    
    private class ApplicationSummary
    {
        public string Title { get; set; } = "";
        public int StatusId { get; set; } = 0;
    }
    
    private class ProjectSummary
    {
        public string Number { get; set; } = "";
        public List<string> Title { get; set; } = [];
    }
    
    private static readonly Func<ApplicationDbContext, int, string> GetMessageReceiverName = 
        EF.CompileQuery((ApplicationDbContext context, int userId) => 
            context.Users
                .TagWith("GetMessageReceiverName")
                .Where(x => x.Id == userId)
                .Select(x => x.FullName)
                .FirstOrDefault() ?? "");
    
    private static readonly Func<ApplicationDbContext, int, Application> GetApplication = 
        EF.CompileQuery((ApplicationDbContext context, int applicationId) => 
            context.Applications
                .TagWith("GetApplication")
                .FirstOrDefault(x => x.Id == applicationId) ?? new Application());
    
    private static readonly Func<ApplicationDbContext, int, ProjectSummary> GetProject = 
        EF.CompileQuery((ApplicationDbContext context, int projectId) => 
            context.Projects
                .TagWith("GetProject")
                .Where(x => x.Id == projectId)
                .Select(x => new ProjectSummary { Title = x.Title, Number = x.Number})
                .FirstOrDefault() ?? new ProjectSummary { Title = new List<string>(), Number = ""});
    
    private static readonly Func<ApplicationDbContext, int, string> GetOrganization = 
        EF.CompileQuery((ApplicationDbContext context, int organizationId) => 
            context.Organizations
                .TagWith("GetOrganization")
                .Where(x => x.Id == organizationId)
                .Select(x => x.Name)
                .FirstOrDefault() ?? "");
    
}