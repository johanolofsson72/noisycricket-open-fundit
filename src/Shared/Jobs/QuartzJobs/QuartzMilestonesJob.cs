using System.Diagnostics;
using System.Net;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Shared.Data.DbContext;
using Shared.Statistics.Services;

namespace Shared.Jobs.QuartzJobs;

public class QuartzMilestonesJob(IServiceScopeFactory serviceScopeFactory) : IJob
{
    public static readonly JobKey Key = new JobKey("QuartzMilestones", "SingleJob");
    
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
            Console.WriteLine($"<= QuartzMilestones started" + $", at: {DateTime.UtcNow:hh:mm:ss}");

            // Get Milestones
            var applicationIds = await context.Applications
                .AsNoTracking()
                .Where(x => x.StatusId != 19)
                .Select(x => new 
                { 
                    x.Id
                })
                .ToListAsync();

            // Update Milestones
            foreach (var application in applicationIds)
            {
                var milestones = await context.Milestones
                    .AsNoTracking()
                    .Where(x => x.ApplicationId == application.Id && x.StatusId != 19)
                    .Select(x => new 
                        { 
                            x.Id, 
                            x.Amount, 
                            x.ExpireDate, 
                            x.IsLocked,
                            x.Requirements,
                            x.Payments 
                        })
                    .ToListAsync();

                var milestoneTotalPayments = 0.0m;
                foreach (var milestone in milestones)
                {
                    var requirementsCount = milestone.Requirements.Count();
                    var milestoneLocked = milestone.Requirements.Any(x => x.IsApproved);
                    var requirementsApproved = milestone.Requirements.All(x => x.IsApproved);
                    var requirementsExpired = milestone.Requirements.All(x => x.ExpireDate < DateTime.UtcNow);
                    var totalPayments = milestone.Payments.Sum(x => x.Amount);
                    
                    milestoneTotalPayments += totalPayments;

                    await context.Milestones
                        .Where(x => x.Id == milestone.Id)
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(p => p.IsLocked, milestoneLocked)
                            .SetProperty(p => p.RequirementsCount, requirementsCount)
                            .SetProperty(p => p.RequirementsApproved, requirementsApproved)
                            .SetProperty(p => p.RequirementsExpired, requirementsExpired)
                            .SetProperty(p => p.TotalPayments, totalPayments));
                }
                
                // Update Application
                await context.Applications
                    .Where(x => x.Id == application.Id)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(p => p.MilestonePayoutTotalAmount, milestoneTotalPayments));
                
                milestoneTotalPayments = 0.0m;
            }
            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString());
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Milestones.ToDescriptionString());
            
            Console.WriteLine($"<= QuartzMilestones succeeded" + $", total processing time: {timer.Elapsed:mm\\:ss}");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("<= QuartzMilestones failed " + ex);
        }
    }
    
}