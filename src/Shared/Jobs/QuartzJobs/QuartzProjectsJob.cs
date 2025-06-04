using System.Diagnostics;
using System.Net;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Shared.Data.DbContext;
using Shared.Statistics.Services;

namespace Shared.Jobs.QuartzJobs;

public class QuartzProjectsJob(IServiceScopeFactory serviceScopeFactory) : IJob
{
    public static readonly JobKey Key = new JobKey("QuartzProjects", "SingleJob");
    
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
            Console.WriteLine($"<= QuartzProjects started" + $", at: {DateTime.UtcNow:hh:mm:ss}");

            // Get all projects
            var projects = await context.Projects
                .AsTracking()
                .Where(x => x.Id != 0)
                .ToListAsync();
            
            foreach (var project in projects)
            {
                var organizationId = project.Organization.OrganizationIdentifier;

                var organization = await context.Organizations
                    .AsNoTracking()
                    .Where(x => x.Id == organizationId)
                    .FirstOrDefaultAsync();

                if (organization == null) continue;

                project.Organization.OrganizationName = organization.Name;
                project.Organization.OrganizationVat = organization.Vat;
                project.Organization.OrganizationEmail = organization.Mail;
                project.Organization.OrganizationUrl = organization.Url;
                project.Organization.OrganizationAddress = organization.Addresses.FirstOrDefault()?.Line1 ?? string.Empty;
                project.Organization.OrganizationCity = organization.Addresses.FirstOrDefault()?.City ?? string.Empty;
                project.Organization.OrganizationPostalCode = organization.Addresses.FirstOrDefault()?.PostalCode ?? string.Empty;
                project.Organization.OrganizationCountry = organization.Addresses.FirstOrDefault()?.Country ?? string.Empty;
                project.Organization.OrganizationPhoneNumber = organization.PhoneNumbers.FirstOrDefault()?.Number ?? string.Empty;

            }

            context.Projects.UpdateRange(projects);
            await context.SaveChangesAsync();
            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString());
            
            Console.WriteLine($"<= QuartzProjects succeeded" + $", total processing time: {timer.Elapsed:mm\\:ss}");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("<= QuartzProjects failed " + ex);
        }
    }
    
}