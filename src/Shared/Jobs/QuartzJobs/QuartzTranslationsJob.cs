using System.Diagnostics;
using System.Net;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Shared.Data.DbContext;
using Shared.Statistics.Services;

namespace Shared.Jobs.QuartzJobs;

public class QuartzTranslationsJob(IServiceScopeFactory serviceScopeFactory) : IJob
{
    public static readonly JobKey Key = new JobKey("QuartzTranslations", "SingleJob");
    
    public async Task Execute(IJobExecutionContext jobExecutionContext)
    {
        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            await using var context = await factory.CreateDbContextAsync(jobExecutionContext.CancellationToken);
            var httpFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

            // Start timer
            var timer = Stopwatch.StartNew();
            Console.WriteLine($"<= QuartzTranslations started" + $", at: {DateTime.UtcNow:hh:mm:ss}");
            
            await TranslateActionTypes(context, httpFactory);
            await TranslateSections(context, httpFactory);
            await TranslateStatuses(context, httpFactory);
            await TranslateApplicationBudgetTypes(context, httpFactory);
            await TranslateDocumentTypes(context, httpFactory);
            await TranslateDocumentDeliveryTypes(context, httpFactory);
            await TranslateGenders(context, httpFactory);
            await TranslateMessageTypes(context, httpFactory);
            await TranslateMilestoneRequirementTypes(context, httpFactory);
            await TranslatePhoneNumberTypes(context, httpFactory);
            await TranslateReactionTypes(context, httpFactory);
            await TranslateSystemMessageDestinations(context, httpFactory);
            
            Console.WriteLine($"<= QuartzTranslations succeeded" + $", total processing time: {timer.Elapsed:mm\\:ss}");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine("<= QuartzTranslations failed " + ex);
        }
    }
    
    
    private async Task TranslateSystemMessageDestinations(ApplicationDbContext context, IHttpClientFactory httpFactory)
    {
        var systemMessageDestinations = await context.SystemMessageDestinations
            .ToListAsync();

        foreach (var systemMessageDestination in systemMessageDestinations)
        {
            var names = systemMessageDestination.Names;

            if (await TranslateX(names, httpFactory)) continue;
                
            systemMessageDestination.Names = names;
            context.SystemMessageDestinations.Update(systemMessageDestination);
        }
        await context.SaveChangesAsync();
    }

    private async Task TranslateReactionTypes(ApplicationDbContext context, IHttpClientFactory httpFactory)
    {
        var reactionTypes = await context.ReactionTypes
            .ToListAsync();

        foreach (var reactionType in reactionTypes)
        {
            var names = reactionType.Names;

            if (await TranslateX(names, httpFactory)) continue;
                
            reactionType.Names = names;
            context.ReactionTypes.Update(reactionType);
        }
        await context.SaveChangesAsync();
    }

    private async Task TranslatePhoneNumberTypes(ApplicationDbContext context, IHttpClientFactory httpFactory)
    {
        var phoneNumberTypes = await context.PhoneNumberTypes
            .ToListAsync();

        foreach (var phoneNumberType in phoneNumberTypes)
        {
            var names = phoneNumberType.Names;

            if (await TranslateX(names, httpFactory)) continue;
                
            phoneNumberType.Names = names;
            context.PhoneNumberTypes.Update(phoneNumberType);
        }
        await context.SaveChangesAsync();
    }

    private async Task TranslateMilestoneRequirementTypes(ApplicationDbContext context, IHttpClientFactory httpFactory)
    {
        var milestoneRequirementTypes = await context.MilestoneRequirementTypes
            .ToListAsync();

        foreach (var milestoneRequirementType in milestoneRequirementTypes)
        {
            var names = milestoneRequirementType.Names;

            if (await TranslateX(names, httpFactory)) continue;
                
            milestoneRequirementType.Names = names;
            context.MilestoneRequirementTypes.Update(milestoneRequirementType);
        }
        await context.SaveChangesAsync();
    }

    private async Task TranslateMessageTypes(ApplicationDbContext context, IHttpClientFactory httpFactory)
    {
        var messageTypes = await context.MessageTypes
            .ToListAsync();

        foreach (var messageType in messageTypes)
        {
            var names = messageType.Names;

            if (await TranslateX(names, httpFactory)) continue;
                
            messageType.Names = names;
            context.MessageTypes.Update(messageType);
        }
        await context.SaveChangesAsync();
    }

    private async Task TranslateGenders(ApplicationDbContext context, IHttpClientFactory httpFactory)
    {
        var genders = await context.Genders
            .ToListAsync();

        foreach (var gender in genders)
        {
            var names = gender.Names;

            if (await TranslateX(names, httpFactory)) continue;
                
            gender.Names = names;
            context.Genders.Update(gender);
        }
        await context.SaveChangesAsync();
    }

    private async Task TranslateDocumentDeliveryTypes(ApplicationDbContext context, IHttpClientFactory httpFactory)
    {
        var documentDeliveryTypes = await context.DocumentDeliveryTypes
            .ToListAsync();

        foreach (var documentDeliveryType in documentDeliveryTypes)
        {
            var names = documentDeliveryType.Names;

            if (await TranslateX(names, httpFactory)) continue;
                
            documentDeliveryType.Names = names;
            context.DocumentDeliveryTypes.Update(documentDeliveryType);
        }
        await context.SaveChangesAsync();
    }

    private async Task TranslateDocumentTypes(ApplicationDbContext context, IHttpClientFactory httpFactory)
    {
        var documentTypes = await context.DocumentTypes
            .ToListAsync();

        foreach (var documentType in documentTypes)
        {
            var names = documentType.Names;

            if (await TranslateX(names, httpFactory)) continue;
                
            documentType.Names = names;
            context.DocumentTypes.Update(documentType);
        }
        await context.SaveChangesAsync();
    }

    private async Task TranslateApplicationBudgetTypes(ApplicationDbContext context, IHttpClientFactory httpFactory)
    {
        var applicationBudgetTypes = await context.ApplicationBudgetTypes
            .ToListAsync();

        foreach (var applicationBudgetType in applicationBudgetTypes)
        {
            var names = applicationBudgetType.Names;

            if (await TranslateX(names, httpFactory)) continue;
                
            applicationBudgetType.Names = names;
            context.ApplicationBudgetTypes.Update(applicationBudgetType);
        }
        await context.SaveChangesAsync();
    }

    private async Task TranslateStatuses(ApplicationDbContext context, IHttpClientFactory httpFactory)
    {
        var statuses = await context.Statuses
            .ToListAsync();

        foreach (var status in statuses)
        {
            var names = status.Names;

            if (await TranslateX(names, httpFactory)) continue;
                
            status.Names = names;
            context.Statuses.Update(status);
        }
        await context.SaveChangesAsync();
    }

    private async Task TranslateSections(ApplicationDbContext context, IHttpClientFactory httpFactory)
    {
        var sections = await context.Sections
            .ToListAsync();

        foreach (var section in sections)
        {
            var names = section.Names;

            if (await TranslateX(names, httpFactory)) continue;
                
            section.Names = names;
            context.Sections.Update(section);
        }
        await context.SaveChangesAsync();
    }

    private async Task TranslateActionTypes(ApplicationDbContext context, IHttpClientFactory httpFactory)
    {
        var actionTypes = await context.ActionTypes
            .ToListAsync();

        foreach (var actionType in actionTypes)
        {
            var names = actionType.Names;

            if (await TranslateX(names, httpFactory)) continue;
                
            actionType.Names = names;
            context.ActionTypes.Update(actionType);
        }
        await context.SaveChangesAsync();
    }

    private async Task<bool> TranslateX(List<string> names, IHttpClientFactory httpFactory)
    {
        if (names.Count < 8) return true;
        if (names.All(x => x.Length > 1)) return true;
        
        if (names[0].Length > 0)
        {
            // översätt från svenska
            var result = await TranslateWithAzure(httpFactory, names[0], "sv", [ "en", "da", "de", "es", "fr", "it", "no" ]);
            if (result is null) return true;
                    
            names[1] = result.Translations.First().Text;
            names[2] = result.Translations.Skip(1).First().Text;
            names[3] = result.Translations.Skip(2).First().Text;
            names[4] = result.Translations.Skip(3).First().Text;
            names[5] = result.Translations.Skip(4).First().Text;
            names[6] = result.Translations.Skip(5).First().Text;
            names[7] = result.Translations.Skip(6).First().Text;
        }
        else if (names[1].Length > 0)
        {
            // översätt från engelska
            var result = await TranslateWithAzure(httpFactory, names[1], "en", [ "sv", "da", "de", "es", "fr", "it", "no" ]);
            if (result is null) return true;
                    
            names[0] = result.Translations.First().Text;
            names[2] = result.Translations.Skip(1).First().Text;
            names[3] = result.Translations.Skip(2).First().Text;
            names[4] = result.Translations.Skip(3).First().Text;
            names[5] = result.Translations.Skip(4).First().Text;
            names[6] = result.Translations.Skip(5).First().Text;
            names[7] = result.Translations.Skip(6).First().Text;
        }
        else if (names[2].Length > 0)
        {
            // översätt från danska
            var result = await TranslateWithAzure(httpFactory, names[2], "da", [ "sv", "en", "de", "es", "fr", "it", "no" ]);
            if (result is null) return true;
                    
            names[0] = result.Translations.First().Text;
            names[1] = result.Translations.Skip(1).First().Text;
            names[3] = result.Translations.Skip(2).First().Text;
            names[4] = result.Translations.Skip(3).First().Text;
            names[5] = result.Translations.Skip(4).First().Text;
            names[6] = result.Translations.Skip(5).First().Text;
            names[7] = result.Translations.Skip(6).First().Text;
        }
        else if (names[3].Length > 0)
        {
            // översätt från tyska
            var result = await TranslateWithAzure(httpFactory, names[3], "de", [ "sv", "en", "da", "es", "fr", "it", "no" ]);
            if (result is null) return true;
                    
            names[0] = result.Translations.First().Text;
            names[1] = result.Translations.Skip(1).First().Text;
            names[2] = result.Translations.Skip(2).First().Text;
            names[4] = result.Translations.Skip(3).First().Text;
            names[5] = result.Translations.Skip(4).First().Text;
            names[6] = result.Translations.Skip(5).First().Text;
            names[7] = result.Translations.Skip(6).First().Text;
        }
        else if (names[4].Length > 0)
        {
            // översätt från spanska
            var result = await TranslateWithAzure(httpFactory, names[4], "es", [ "sv", "en", "da", "de", "fr", "it", "no" ]);
            if (result is null) return true;
                    
            names[0] = result.Translations.First().Text;
            names[1] = result.Translations.Skip(1).First().Text;
            names[2] = result.Translations.Skip(2).First().Text;
            names[3] = result.Translations.Skip(3).First().Text;
            names[5] = result.Translations.Skip(4).First().Text;
            names[6] = result.Translations.Skip(5).First().Text;
            names[7] = result.Translations.Skip(6).First().Text;
        }
        else if (names[5].Length > 0)
        {
            // översätt från franska
            var result = await TranslateWithAzure(httpFactory, names[5], "fr", [ "sv", "en", "da", "de", "es", "it", "no" ]);
            if (result is null) return true;
                    
            names[0] = result.Translations.First().Text;
            names[1] = result.Translations.Skip(1).First().Text;
            names[2] = result.Translations.Skip(2).First().Text;
            names[3] = result.Translations.Skip(3).First().Text;
            names[4] = result.Translations.Skip(4).First().Text;
            names[6] = result.Translations.Skip(5).First().Text;
            names[7] = result.Translations.Skip(6).First().Text;
        }
        else if (names[6].Length > 0)
        {
            // översätt från italienska
            var result = await TranslateWithAzure(httpFactory, names[6], "it", [ "sv", "en", "da", "de", "es", "fr", "no" ]);
            if (result is null) return true;
                    
            names[0] = result.Translations.First().Text;
            names[1] = result.Translations.Skip(1).First().Text;
            names[2] = result.Translations.Skip(2).First().Text;
            names[3] = result.Translations.Skip(3).First().Text;
            names[4] = result.Translations.Skip(4).First().Text;
            names[5] = result.Translations.Skip(5).First().Text;
            names[7] = result.Translations.Skip(6).First().Text;
        }
        else if (names[7].Length > 0)
        {
            // översätt från norska
            var result = await TranslateWithAzure(httpFactory, names[7], "no", [ "sv", "en", "da", "de", "es", "fr", "it" ]);
            if (result is null) return true;
                    
            names[0] = result.Translations.First().Text;
            names[1] = result.Translations.Skip(1).First().Text;
            names[2] = result.Translations.Skip(2).First().Text;
            names[3] = result.Translations.Skip(3).First().Text;
            names[4] = result.Translations.Skip(4).First().Text;
            names[5] = result.Translations.Skip(5).First().Text;
            names[6] = result.Translations.Skip(6).First().Text;
        }

        return false;
    }

    private async Task<TranslationResponse?> TranslateWithAzure(IHttpClientFactory httpFactory, string textToTranslate, string from, string[] to)
    {
        try
        {
            var translateWords = new TranslateWords
            {
                TextToTranslate = textToTranslate,
                From = from,
                To = to
            };
            var httpClient = httpFactory.CreateClient("openai");
            var response = await httpClient.PostAsJsonAsync("/api/v1/translateWords", translateWords);
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<List<TranslationResponse>>(jsonString);
            return json?.First();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }
    
    private class TranslationResponse
    {
        public List<Translation> Translations { get; set; } = [];
    }

    private class Translation
    {
        public string Text { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
    }

    public class TranslateWords
    {
        public required string TextToTranslate { get; set; }
        public required string From { get; set; }
        public required string[] To { get; set; }
    }

    
}