using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Quartz;
using Shared.Data.DbContext;
using Shared.OpenAi.Entities;
using Shared.Statistics.Services;

namespace Shared.Jobs.QuartzJobs;

public class QuartzOpenAiUsersJob(IServiceScopeFactory serviceScopeFactory) : IJob
{
    public static readonly JobKey Key = new JobKey("QuartzOpenAiUsers", "SingleJob");

    public async Task Execute(IJobExecutionContext jobExecutionContext)
    {
        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            await using var context = await factory.CreateDbContextAsync(jobExecutionContext.CancellationToken);

            // Start timer
            var timer = Stopwatch.StartNew();
            Console.WriteLine($"<= QuartzOpenAiUsers started" + $", at: {DateTime.UtcNow:hh:mm:ss}");

            // Delete all OpenAiUsers
            await context.OpenAiUsers.Where(x => x.Id > 0).ExecuteDeleteAsync();
            
            // Get all users and their organizations
            var temp = await context.Users
                .AsNoTracking()
                .Select(x => new 
                {
                    Id = x.Id,
                    Name = x.FirstName + " " + x.LastName,
                    Organizations = x.Organizations
                })
                .ToListAsync();
            
            // Create OpenAiUsers
            var openAiUsers = temp
                .Select(x => new OpenAiUser
                {
                    UserId = x.Id,
                    UserName = x.Name,
                    Organizations = x.Organizations.Select(f => new OpenAiUserOrganization()
                    {
                        Id = f.OrganizationIdentifier,
                        Name = f.OrganizationName
                    }).ToList(),
                    ExpireDate = DateTime.Now.AddMonths(1)
                })
                .ToList();
            
            // Add OpenAiUsers to database
            await context.OpenAiUsers.AddRangeAsync(openAiUsers);
            
            // Save changes
            await context.SaveChangesAsync();

            Console.WriteLine($"<= QuartzOpenAiUsers succeeded" + $", total processing time: {timer.Elapsed:mm\\:ss}");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("<= QuartzOpenAiUsers failed " + ex);
        }
    }
    
}