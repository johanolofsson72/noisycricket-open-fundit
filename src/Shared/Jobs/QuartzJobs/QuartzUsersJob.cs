using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Quartz;
using Shared.Data.DbContext;
using Shared.Statistics.Services;

namespace Shared.Jobs.QuartzJobs;

public class QuartzUsersJob(IServiceScopeFactory serviceScopeFactory) : IJob
{
    public static readonly JobKey Key = new JobKey("QuartzUsers", "SingleJob");

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
            Console.WriteLine($"<= QuartzUsers started" + $", at: {DateTime.UtcNow:hh:mm:ss}");

            // Get all users
            var users = await context.Users
                .AsTracking()
                .Where(x => x.StatusId != 19)
                .ToListAsync();

            // Get message count and organization summary for each user
            foreach (var user in users)
            {
                var messageCount = await GetMessageCount(context, user.Id, 24);

                user.MessageCount = messageCount;

                foreach (var org in user.Organizations)
                {
                    var organization = await GetOrganizationSummary(context, org.OrganizationIdentifier);
                    
                    if (organization is null) continue;
                    
                    org.OrganizationName = organization.Name;
                    org.OrganizationVat = organization.Vat;
                    
                }
            }
            
            // Save changes
            await context.SaveChangesAsync();
            
            // Clear cache
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Users.ToDescriptionString());
            
            Console.WriteLine($"<= QuartzUsers succeeded" + $", total processing time: {timer.Elapsed:mm\\:ss}");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("<= QuartzUsers failed " + ex);
        }
    }
    
    private static readonly Func<ApplicationDbContext, int, int, Task<int>> GetMessageCount = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int userId, int statusId) => 
            context.Messages
                .TagWith("GetMessageCount")
                .Count(x => x.Receiver.ContactIdentifier == userId && x.StatusId == statusId));
    
    private static readonly Func<ApplicationDbContext, int, Task<OrganizationSummary?>> GetOrganizationSummary = 
        EF.CompileAsyncQuery((ApplicationDbContext context, int organizationId) => 
            context.Organizations
                .TagWith("GetOrganizationSummary")
                .Where(x => x.Id == organizationId)
                .Select(x => new OrganizationSummary(x.Name, x.Vat))
                .FirstOrDefault());
    
    private record OrganizationSummary(string Name, string Vat);

    
}