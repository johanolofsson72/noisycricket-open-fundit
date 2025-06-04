using System.Diagnostics;
using System.Net;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Shared.Controls.Entities;
using Shared.Data.DbContext;
using Shared.Global.Entities;
using Shared.Statistics.Services;

namespace Shared.Jobs.QuartzJobs;

public class QuartzOrganizationsJob(IServiceScopeFactory serviceScopeFactory) : IJob
{
    public static readonly JobKey Key = new JobKey("QuartzOrganizations", "SingleJob");
    
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
            Console.WriteLine($"<= QuartzOrganizations started" + $", at: {DateTime.UtcNow:hh:mm:ss}");

            // Get Organizations
            var organizations = await context.Organizations
                .AsTracking()
                .Where(x => x.StatusId != 19)
                .ToListAsync();
            
            // Get all the data
            var currencies = (await context.Currencies.AsNoTracking().ToListAsync()).ToDto().ToList();
            var statuses = (await context.Statuses.AsNoTracking().ToListAsync()).ToDto().ToList();
            var sections = (await context.Sections.AsNoTracking().ToListAsync()).ToDto().ToList();
            var actionTypes = (await context.ActionTypes.AsNoTracking().ToListAsync()).ToDto().ToList();
            var claimTypes= (await context.ClaimTypes.AsNoTracking().ToListAsync()).ToDto().ToList();
            var eventTypes = (await context.EventTypes.AsNoTracking().ToListAsync()).ToDto().ToList();
            var genders = (await context.Genders.AsNoTracking().ToListAsync()).ToDto().ToList();
            var messageTypes = (await context.MessageTypes.AsNoTracking().ToListAsync()).ToDto().ToList();
            var phoneNumberTypes = (await context.PhoneNumberTypes.AsNoTracking().ToListAsync()).ToDto().ToList();
            var reactionTypes = (await context.ReactionTypes.AsNoTracking().ToListAsync()).Select(x => x.ToDto()).ToList();
            var systemMessageDestinations = (await context.SystemMessageDestinations.AsNoTracking().ToListAsync()).ToDto().ToList();;
            var milestoneRequirementTypes = (await context.MilestoneRequirementTypes.AsNoTracking().ToListAsync()).ToDto().ToList();
            var controlTypes = (await context.ControlTypes.AsNoTracking().ToListAsync()).Select(x => x.ToDto()).ToList();
            var documentTypes = (await context.DocumentTypes.AsNoTracking().ToListAsync()).ToDto();
            var documentDeliveryTypes = (await context.DocumentDeliveryTypes.AsNoTracking().ToListAsync()).ToDto();
            var applicationBudgetTypes = (await context.ApplicationBudgetTypes.AsNoTracking().ToListAsync()).ToDto();

            // Add the data to the organizations
            foreach (var organization in organizations)
            {
                organization.Currencies = currencies.Select(x => new OrganizationCurrencyDto
                { 
                    Name = x.Name,
                    Rate = x.Rate,
                    CreatedDate = x.CreatedDate
                }).ToList();
                organization.Statuses = statuses.Select(x => new OrganizationStatusDto() { OrganizationStatusIdentifier = x.Id, Names = x.Names }).ToList();
                organization.Sections = sections.Select(x => new OrganizationSectionDto() { OrganizationSectionIdentifier = x.Id, Names = x.Names, Order = x.Order, Enabled = x.Enabled}).ToList();
                organization.ActionTypes = actionTypes.Select(x => new OrganizationActionTypeDto() { OrganizationActionTypeIdentifier = x.Id, Names = x.Names }).ToList();
                organization.ClaimTypes= claimTypes.Select(x => new OrganizationClaimTypeDto() { OrganizationClaimTypeIdentifier = x.Id, Names = x.Names, Tag = x.Tag}).ToList();
                organization.EventTypes = eventTypes.Select(x => new OrganizationEventTypeDto() { OrganizationEventTypeIdentifier = x.Id, Names = x.Names }).ToList();
                organization.Genders = genders.Select(x => new OrganizationGenderDto() { OrganizationGenderIdentifier = x.Id, Names = x.Names }).ToList();
                organization.MessageTypes = messageTypes.Select(x => new OrganizationMessageTypeDto() { OrganizationMessageTypeIdentifier = x.Id, Names = x.Names }).ToList();
                organization.PhoneNumberTypes = phoneNumberTypes.Select(x => new OrganizationPhoneNumberTypeDto() { OrganizationPhoneNumberTypeIdentifier = x.Id, Names = x.Names }).ToList();
                organization.ReactionTypes = reactionTypes.Select(x => new OrganizationReactionTypeDto() { OrganizationReactionTypeIdentifier = x.Id, Names = x.Names, Messages = x.Messages }).ToList();
                organization.SystemMessageDestinations = systemMessageDestinations.Select(x => new OrganizationSystemMessageDestinationDto() { OrganizationSystemMessageDestinationIdentifier = x.Id, Names = x.Names }).ToList();
                organization.MilestoneRequirementTypes = milestoneRequirementTypes.Select(x => new OrganizationMilestoneRequirementTypeDto() { OrganizationMilestoneRequirementTypeIdentifier = x.Id, Names = x.Names }).ToList();
                organization.ControlTypes = controlTypes.Select(x => new OrganizationControlTypeDto() { OrganizationControlTypeIdentifier = x.Id, Name = x.Name }).ToList();
                organization.DocumentTypes = documentTypes.Select(x => new OrganizationDocumentTypeDto() { OrganizationDocumentTypeIdentifier = x.Id, Names = x.Names }).ToList();
                organization.DocumentDeliveryTypes = documentDeliveryTypes.Select(x => new OrganizationDocumentDeliveryTypeDto() { OrganizationDocumentDeliveryTypeIdentifier = x.Id, Names = x.Names }).ToList();
                organization.ApplicationBudgetTypes = applicationBudgetTypes.Select(x => new OrganizationApplicationBudgetTypeDto() { OrganizationApplicationBudgetTypeIdentifier = x.Id, Names = x.Names }).ToList();
                
            }

            // Save the organizations
            context.Organizations.UpdateRange(organizations);
            await context.SaveChangesAsync();
            
            // Clear cache
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Organizations.ToDescriptionString());
            
            Console.WriteLine($"<= QuartzOrganizations succeeded" + $", total processing time: {timer.Elapsed:mm\\:ss}");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("<= QuartzOrganizations failed " + ex);
        }
    }
    
}