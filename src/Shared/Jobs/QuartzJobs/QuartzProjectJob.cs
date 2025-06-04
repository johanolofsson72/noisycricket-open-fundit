using System.Diagnostics;
using System.Net;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Shared.Data.DbContext;
using Shared.Statistics.Services;

namespace Shared.Jobs.QuartzJobs;

public class QuartzProjectJob(IServiceScopeFactory serviceScopeFactory) : IJob
{
    public static readonly JobKey Key = new JobKey("QuartzProject", "SingleJob");
    
    public async Task Execute(IJobExecutionContext jobExecutionContext)
    {
        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            await using var context = await factory.CreateDbContextAsync(jobExecutionContext.CancellationToken);
            var cache = scope.ServiceProvider.GetRequiredService<IEasyCachingProvider>();
            var data = jobExecutionContext.JobDetail.JobDataMap;
            var projectId = data.GetInt("projectId");

            // Start timer
            var timer = Stopwatch.StartNew();
            Console.WriteLine($"<= QuartzProject started for id: {projectId}" + $", at: {DateTime.UtcNow:hh:mm:ss}");

            // Kontrollera om projektet finns med retry-logik
            var retryCount = 3;
            var delay = 500; // Millisekunder
            var project = await TryGetProjectWithRetry(context, projectId, retryCount, delay);

            if (project == null)
            {
                throw new Exception($"Project with id {projectId} could not be found after {retryCount} attempts");
            }

            // Hämta organisationen
            var organizationId = project.Organization.OrganizationIdentifier;

            var organization = await context.Organizations
                .AsNoTracking()
                .Where(x => x.Id == organizationId)
                .FirstOrDefaultAsync();

            if (organization == null) throw new Exception("Organization not found");

            project.Organization.OrganizationName = organization.Name;
            project.Organization.OrganizationVat = organization.Vat;
            project.Organization.OrganizationEmail = organization.Mail;
            project.Organization.OrganizationUrl = organization.Url;
            project.Organization.OrganizationAddress = organization.Addresses.FirstOrDefault()?.Line1 ?? string.Empty;
            project.Organization.OrganizationCity = organization.Addresses.FirstOrDefault()?.City ?? string.Empty;
            project.Organization.OrganizationPostalCode = organization.Addresses.FirstOrDefault()?.PostalCode ?? string.Empty;
            project.Organization.OrganizationCountry = organization.Addresses.FirstOrDefault()?.Country ?? string.Empty;
            project.Organization.OrganizationPhoneNumber = organization.PhoneNumbers.FirstOrDefault()?.Number ?? string.Empty;

            context.Projects.Update(project);
            await context.SaveChangesAsync();
            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString());
            
            Console.WriteLine($"<= QuartzProject succeeded for id: {projectId}" + $", total processing time: {timer.Elapsed:mm\\:ss}");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("<= QuartzProject failed " + ex);
        }
    }
    
    private async Task<Project?> TryGetProjectWithRetry(ApplicationDbContext context, int projectId, int maxRetries, int delayMs)
    {
        for (var attempt = 0; attempt < maxRetries; attempt++)
        {
            var project = await context.Projects
                .AsTracking()
                .Where(x => x.Id == projectId)
                .FirstOrDefaultAsync();

            if (project != null)
            {
                return project;
            }

            if (attempt < maxRetries - 1)
            {
                await Task.Delay(delayMs);
            }
        }

        return null;
    }

}