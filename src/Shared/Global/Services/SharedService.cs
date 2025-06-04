using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Shared.Applications.DTOs;
using Shared.Applications.Entities;
using Shared.Controls.DTOs;
using Shared.Controls.Entities;
using Shared.Data.DbContext;
using Shared.Documents.DTOs;
using Shared.Documents.Entities;
using Shared.Global.DTOs;
using Shared.Global.Entities;
using Shared.Global.Structs;
using Shared.Jobs.QuartzJobs;
using Shared.Milestones.DTOs;
using Shared.Milestones.Entities;

namespace Shared.Global.Services;

public class SharedService(ApplicationDbContext context, IConfiguration configuration, ISchedulerFactory schedulerFactory)
{
    private static readonly List<int> _allProjectStatuses = [3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18];
    private static readonly List<int> _supportedProjectStatuses = [10, 11, 12, 13, 14, 15, 16, 17, 18];
    private static readonly List<int> _notActiveProjectStatuses = [3, 4, 5, 6, 7, 8, 9];
    
    // CultureToIndex
    public enum CultureToIndex
    {
        [Description("sv-SE")] Swedish = 0,
        [Description("en-US")] English = 1,
        [Description("da-DK")] Danish = 2,
        [Description("de-DE")] German = 3,
        [Description("es-ES")] Spanish = 4,
        [Description("fr-FR")] French = 5,
        [Description("it-IT")] Italian = 6,
        [Description("nb-NO")] Norwegian = 7
    }

    public int IndexByCulture()
    {
        return Thread.CurrentThread.CurrentCulture.Name switch
        {
            "sv-SE" => 0,
            "en-US" => 1,
            "da-DK" => 2,
            "de-DE" => 3,
            "es-ES" => 4,
            "fr-FR" => 5,
            "it-IT" => 6,
            "nb-NO" => 7,
            _ => 0
        };
    }

    public string ValueByCulture(List<string> values)
    {
        return Thread.CurrentThread.CurrentCulture.Name switch
        {
            "sv-SE" => values[0],
            "en-US" => values[1],
            "da-DK" => values[2],
            "de-DE" => values[3],
            "es-ES" => values[4],
            "fr-FR" => values[5],
            "it-IT" => values[6],
            "nb-NO" => values[7],
            _ => values[0]
        };
    }
    
    public async Task ExecuteTranslationJob()
    {
        var scheduler = await schedulerFactory.GetScheduler();
        
        // QuartzTranslationsJob
        var quartzTranslationsJob = JobBuilder.Create<QuartzTranslationsJob>()
            .WithIdentity($"QuartzTranslationsJob", "SingleJob")
            .Build();
        
        var quartzTranslationsTrigger = TriggerBuilder.Create()
            .WithIdentity($"QuartzTranslationsTrigger", "SingleTrigger")
            .StartNow()
            .Build();

        await scheduler.ScheduleJob(quartzTranslationsJob, quartzTranslationsTrigger, new CancellationToken());
    }
    
    // Event types
    public async Task<Result<IEnumerable<EventTypeDto>, Exception>> GetAllEventTypesAsync()
    {
        try
        {
            var eventTypes = GetAllEventTypes(context) ?? throw new Exception("Event types not found");
            return await eventTypes.ToListAsync();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<EventTypeDto, Exception>> CreateEventTypeAsync(CreateEventTypeDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");

            var eventType = new EventType
            {
                Names = dto.Names
            };

            await context.EventTypes.AddAsync(eventType, ct);
            await context.SaveChangesAsync(ct);
            
            return eventType.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> UpdateEventTypeAsync(int eventTypeId, UpdateEventTypeDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");
            
            var eventType = await context.EventTypes
                .Where(x => x.Id == eventTypeId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("EventType not found");
            
            eventType.Names = dto.Names;

            context.EventTypes.Update(eventType);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> DeleteEventTypeAsync(int eventTypeId, CancellationToken ct)
    {
        try
        {
            if (eventTypeId == 0) throw new Exception("eventTypeId is required");
            
            var eventType = await context.EventTypes
                .Where(x => x.Id == eventTypeId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("EventType not found");

            context.EventTypes.Remove(eventType);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    // Action types
    public async Task<Result<IEnumerable<ActionTypeDto>, Exception>> GetAllActionTypesAsync()
    {
        try
        {
            var actionTypes = GetAllActionTypes(context) ?? throw new Exception("Action types not found");
            return await actionTypes.ToListAsync();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<ActionTypeDto, Exception>> CreateActionTypeAsync(CreateActionTypeDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");

            var actionType = new ActionType
            {
                Names = dto.Names
            };

            await context.ActionTypes.AddAsync(actionType, ct);
            await context.SaveChangesAsync(ct);
            
            return actionType.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> UpdateActionTypeAsync(int actionTypeId, UpdateActionTypeDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");
            
            var actionType = await context.ActionTypes
                .Where(x => x.Id == actionTypeId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("ActionType not found");
            
            actionType.Names = dto.Names;

            context.ActionTypes.Update(actionType);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> DeleteActionTypeAsync(int actionTypeId, CancellationToken ct)
    {
        try
        {
            if (actionTypeId == 0) throw new Exception("actionTypeId is required");
            
            var actionType = await context.ActionTypes
                .Where(x => x.Id == actionTypeId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("ActionType not found");

            context.ActionTypes.Remove(actionType);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    // System message destination types
    public async Task<Result<IEnumerable<SystemMessageDestinationDto>, Exception>> GetAllSystemMessageDestinationsAsync()
    {
        try
        {
            var systemMessageDestinationTypes = GetAllSystemMessageDestinationTypes(context) ?? throw new Exception("Statuses not found");
            return await systemMessageDestinationTypes.ToListAsync();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<SystemMessageDestinationDto, Exception>> CreateSystemMessageDestinationAsync(CreateSystemMessageDestinationDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");
            
            var actionType = new ActionType
            {
                Names = dto.Names
            };
            await context.ActionTypes.AddAsync(actionType, ct);
            
            var systemMessageDestination = new SystemMessageDestination
            {
                Names = dto.Names
            };

            await context.SystemMessageDestinations.AddAsync(systemMessageDestination, ct);
            await context.SaveChangesAsync(ct);
            
            return systemMessageDestination.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> UpdateSystemMessageDestinationAsync(int systemMessageDestinationId, UpdateSystemMessageDestinationDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");
            
            var systemMessageDestination = await context.SystemMessageDestinations
                .Where(x => x.Id == systemMessageDestinationId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("SystemMessageDestination not found");
            
            systemMessageDestination.Names = dto.Names;

            context.SystemMessageDestinations.Update(systemMessageDestination);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> DeleteSystemMessageDestinationAsync(int systemMessageDestinationId, CancellationToken ct)
    {
        try
        {
            if (systemMessageDestinationId == 0) throw new Exception("statusId is required");
            
            var status = await context.SystemMessageDestinations
                .Where(x => x.Id == systemMessageDestinationId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("SystemMessageDestination not found");

            context.SystemMessageDestinations.Remove(status);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    // Reaction types
    public async Task<Result<IEnumerable<ReactionTypeDto>, Exception>> GetAllReactionTypesAsync()
    {
        try
        {
            var reactionTypes = GetAllReactionTypes(context) ?? throw new Exception("Statuses not found");
            return await reactionTypes.ToListAsync();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<ReactionTypeDto, Exception>> CreateReactionTypeAsync(CreateReactionTypeDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");

            var reactionType = new ReactionType
            {
                Names = dto.Names,
                Messages = dto.Messages
            };

            await context.ReactionTypes.AddAsync(reactionType, ct);
            await context.SaveChangesAsync(ct);
            
            return reactionType.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> UpdateReactionTypeAsync(int reactionTypeId, UpdateReactionTypeDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");
            
            var reactionType = await context.ReactionTypes
                .Where(x => x.Id == reactionTypeId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("ReactionType not found");
            
            reactionType.Names = dto.Names;
            reactionType.Messages = dto.Messages;

            context.ReactionTypes.Update(reactionType);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> DeleteReactionTypeAsync(int reactionTypeId, CancellationToken ct)
    {
        try
        {
            if (reactionTypeId == 0) throw new Exception("reactionTypeId is required");
            
            var reactionType = await context.ReactionTypes
                .Where(x => x.Id == reactionTypeId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("ReactionType not found");

            context.ReactionTypes.Remove(reactionType);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    // Phone number types
    public async Task<Result<IEnumerable<PhoneNumberTypeDto>, Exception>> GetAllPhoneNumberTypesAsync()
    {
        try
        {
            var phoneNumberTypes = GetAllPhoneNumberTypes(context) ?? throw new Exception("Statuses not found");
            return await phoneNumberTypes.ToListAsync();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<PhoneNumberTypeDto, Exception>> CreatePhoneNumberTypeAsync(CreatePhoneNumberTypeDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Name is required");

            var phoneNumberType = new PhoneNumberType
            {
                Names = dto.Names
            };

            await context.PhoneNumberTypes.AddAsync(phoneNumberType, ct);
            await context.SaveChangesAsync(ct);
            
            return phoneNumberType.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> UpdatePhoneNumberTypeAsync(int phoneNumberTypeId, UpdatePhoneNumberTypeDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");
            
            var phoneNumberType = await context.PhoneNumberTypes
                .Where(x => x.Id == phoneNumberTypeId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("PhoneNumberType not found");
            
            phoneNumberType.Names = dto.Names;

            context.PhoneNumberTypes.Update(phoneNumberType);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> DeletePhoneNumberTypeAsync(int phoneNumberTypeId, CancellationToken ct)
    {
        try
        {
            if (phoneNumberTypeId == 0) throw new Exception("phoneNumberTypeId is required");
            
            var phoneNumberType = await context.PhoneNumberTypes
                .Where(x => x.Id == phoneNumberTypeId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("PhoneNumberType not found");

            context.PhoneNumberTypes.Remove(phoneNumberType);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    // Message types
    public async Task<Result<IEnumerable<MessageTypeDto>, Exception>> GetAllMessageTypesAsync()
    {
        try
        {
            var messageTypes = GetAllMessageTypes(context) ?? throw new Exception("Statuses not found");
            return await messageTypes.ToListAsync();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<MessageTypeDto, Exception>> CreateMessageTypeAsync(CreateMessageTypeDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");

            var messageType = new MessageType
            {
                Names = dto.Names
            };

            await context.MessageTypes.AddAsync(messageType, ct);
            await context.SaveChangesAsync(ct);
            
            return messageType.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> UpdateMessageTypeAsync(int messageTypeId, UpdateMessageTypeDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");
            
            var messageType = await context.MessageTypes
                .Where(x => x.Id == messageTypeId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("MessageType not found");
            
            messageType.Names = dto.Names;

            context.MessageTypes.Update(messageType);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> DeleteMessageTypeAsync(int messageTypeId, CancellationToken ct)
    {
        try
        {
            if (messageTypeId == 0) throw new Exception("messageTypeId is required");
            
            var messageType = await context.MessageTypes
                .Where(x => x.Id == messageTypeId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("MessageType not found");

            context.MessageTypes.Remove(messageType);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    // Claim types
    public async Task<Result<IEnumerable<ClaimTypeDto>, Exception>> GetAllClaimTypesAsync()
    {
        try
        {
            var claimTypes = GetAllClaimTypes(context) ?? throw new Exception("Statuses not found");
            return await claimTypes.ToListAsync();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<ClaimTypeDto, Exception>> CreateClaimTypeAsync(CreateClaimTypeDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");

            var claimType = new ClaimType
            {
                Names = dto.Names,
                Tag = dto.Tag
            };

            await context.ClaimTypes.AddAsync(claimType, ct);
            await context.SaveChangesAsync(ct);
            
            return claimType.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> UpdateClaimTypeAsync(int claimTypeId, UpdateClaimTypeDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");
            
            var claimType = await context.ClaimTypes
                .Where(x => x.Id == claimTypeId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("ClaimType not found");
            
            claimType.Names = dto.Names;
            claimType.Tag = dto.Tag;

            context.ClaimTypes.Update(claimType);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> DeleteClaimTypeAsync(int claimTypeId, CancellationToken ct)
    {
        try
        {
            if (claimTypeId == 0) throw new Exception("statusId is required");
            
            var claimType = await context.ClaimTypes
                .Where(x => x.Id == claimTypeId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("ClaimType not found");

            context.ClaimTypes.Remove(claimType);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    // Genders
    public async Task<Result<IEnumerable<GenderDto>, Exception>> GetAllGendersAsync()
    {
        try
        {
            var genders = GetAllGenders(context) ?? throw new Exception("Genders not found");
            return await genders.ToListAsync();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<GenderDto, Exception>> CreateGenderAsync(CreateGenderDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");

            var gender = new Gender
            {
                Names = dto.Names
            };

            await context.Genders.AddAsync(gender, ct);
            await context.SaveChangesAsync(ct);
            
            return gender.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> UpdateGenderAsync(int genderId, UpdateGenderDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");
            
            var gender = await context.Genders
                .Where(x => x.Id == genderId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Gender not found");
            
            gender.Names = dto.Names;

            context.Genders.Update(gender);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> DeleteGenderAsync(int genderId, CancellationToken ct)
    {
        try
        {
            if (genderId == 0) throw new Exception("statusId is required");
            
            var gender = await context.Genders
                .Where(x => x.Id == genderId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Gender not found");

            context.Genders.Remove(gender);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    // Sections
    public async Task<Result<IEnumerable<SectionDto>, Exception>> GetAllSectionsAsync()
    {
        try
        {
            var sections = GetAllSections(context) ?? throw new Exception("Statuses not found");
            return await sections.ToListAsync();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<SectionDto, Exception>> CreateSectionAsync(CreateSectionDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");

            var section = new Section
            {
                Names = dto.Names,
                Order = dto.Order,
                Enabled = dto.Enabled
            };

            await context.Sections.AddAsync(section, ct);
            await context.SaveChangesAsync(ct);
            
            return section.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> UpdateSectionAsync(int sectionId, UpdateSectionDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");
            
            var section = await context.Sections
                .Where(x => x.Id == sectionId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Section not found");
            
            section.Names = dto.Names;
            section.Order = dto.Order;
            section.Enabled = dto.Enabled;

            context.Sections.Update(section);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> DeleteSectionAsync(int sectionId, CancellationToken ct)
    {
        try
        {
            if (sectionId == 0) throw new Exception("statusId is required");
            
            var section = await context.Sections
                .Where(x => x.Id == sectionId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Section not found");

            context.Sections.Remove(section);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    // Statuses
    public async Task<Result<IEnumerable<StatusDto>, Exception>> GetAllStatusesAsync()
    {
        try
        {
            var statuses = GetAllStatuses(context) ?? throw new Exception("Statuses not found");
            return await statuses.ToListAsync();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<StatusDto, Exception>> CreateStatusAsync(CreateStatusDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");

            var status = new Status
            {
                Names = dto.Names
            };

            await context.Statuses.AddAsync(status, ct);
            await context.SaveChangesAsync(ct);
            
            return status.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> UpdateStatusAsync(int statusId, UpdateStatusDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");
            
            var status = await context.Statuses
                .Where(x => x.Id == statusId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Status not found");
            
            status.Names= dto.Names;

            context.Statuses.Update(status);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> DeleteStatusAsync(int statusId, CancellationToken ct)
    {
        try
        {
            if (statusId == 0) throw new Exception("statusId is required");
            
            var status = await context.Statuses
                .Where(x => x.Id == statusId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("Status not found");

            context.Statuses.Remove(status);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<IEnumerable<StatusDto>, Exception>> GetAllProjectStatusesAsync()
    {
        try
        {
            var statuses = GetAllProjectStatuses(context) ?? throw new Exception("Statuses not found");
            return await statuses.ToListAsync();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<IEnumerable<StatusDto>, Exception>> GetSupportedProjectStatusesAsync()
    {
        try
        {
            var statuses = GetSupportedProjectStatuses(context) ?? throw new Exception("Statuses not found");
            return await statuses.ToListAsync();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<IEnumerable<StatusDto>, Exception>> GetNotActiveProjectStatusesAsync()
    {
        try
        {
            var statuses = GetNotActiveProjectStatuses(context) ?? throw new Exception("Statuses not found");
            return await statuses.ToListAsync();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    // ApplicationBudgetTypes
    public async Task<Result<IEnumerable<ApplicationBudgetTypeDto>, Exception>> GetAllApplicationBudgetTypesAsync()
    {
        try
        {
            var applicationBudgetTypes = GetAllApplicationBudgetTypes(context) ?? throw new Exception("ApplicationBudgetTypes not found");
            return await applicationBudgetTypes.ToListAsync();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<ApplicationBudgetTypeDto, Exception>> CreateApplicationBudgetTypeAsync(CreateApplicationBudgetTypeDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");

            var applicationBudgetType = new ApplicationBudgetType
            {
                Names = dto.Names
            };

            await context.ApplicationBudgetTypes.AddAsync(applicationBudgetType, ct);
            await context.SaveChangesAsync(ct);
            
            return applicationBudgetType.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> UpdateApplicationBudgetTypeAsync(int applicationBudgetTypeId, UpdateApplicationBudgetTypeDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");
            
            var applicationBudgetType = await context.ApplicationBudgetTypes
                .Where(x => x.Id == applicationBudgetTypeId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("ApplicationBudgetType not found");
            
            applicationBudgetType.Names = dto.Names;

            context.ApplicationBudgetTypes.Update(applicationBudgetType);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> DeleteApplicationBudgetTypeAsync(int applicationBudgetTypeId, CancellationToken ct)
    {
        try
        {
            if (applicationBudgetTypeId == 0) throw new Exception("applicationBudgetTypeId is required");
            
            var applicationBudgetType = await context.ApplicationBudgetTypes
                .Where(x => x.Id == applicationBudgetTypeId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("ApplicationBudgetType not found");

            context.ApplicationBudgetTypes.Remove(applicationBudgetType);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    
    // DocumentTypes
    public async Task<Result<IEnumerable<DocumentTypeDto>, Exception>> GetAllDocumentTypesAsync()
    {
        try
        {
            var documentTypes = GetAllDocumentTypes(context) ?? throw new Exception("DocumentDeliveryTypes not found");
            return await documentTypes.ToListAsync();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<DocumentTypeDto, Exception>> CreateDocumentTypeAsync(CreateDocumentTypeDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");

            var documentType = new DocumentType
            {
                Names = dto.Names
            };

            await context.DocumentTypes.AddAsync(documentType, ct);
            await context.SaveChangesAsync(ct);
            
            return documentType.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> UpdateDocumentTypeAsync(int documentTypesId, UpdateDocumentTypeDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");
            
            var documentType = await context.DocumentTypes
                .Where(x => x.Id == documentTypesId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("DocumentType not found");
            
            documentType.Names = dto.Names;

            context.DocumentTypes.Update(documentType);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> DeleteDocumentTypeAsync(int documentTypesId, CancellationToken ct)
    {
        try
        {
            if (documentTypesId == 0) throw new Exception("documentTypesId is required");
            
            var documentType = await context.DocumentTypes
                .Where(x => x.Id == documentTypesId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("DocumentType not found");

            context.DocumentTypes.Remove(documentType);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    
    // DocumentDeliveryTypes
    public async Task<Result<IEnumerable<DocumentDeliveryTypeDto>, Exception>> GetAllDocumentDeliveryTypesAsync()
    {
        try
        {
            var documentDeliveryTypes = GetAllDocumentDeliveryTypes(context) ?? throw new Exception("DocumentDeliveryTypes not found");
            return await documentDeliveryTypes.ToListAsync();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<DocumentDeliveryTypeDto, Exception>> CreateDocumentDeliveryTypeAsync(CreateDocumentDeliveryTypeDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");

            var documentDeliveryType = new DocumentDeliveryType
            {
                Names = dto.Names
            };

            await context.DocumentDeliveryTypes.AddAsync(documentDeliveryType, ct);
            await context.SaveChangesAsync(ct);
            
            return documentDeliveryType.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> UpdateDocumentDeliveryTypeAsync(int documentDeliveryTypeId, UpdateDocumentDeliveryTypeDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");
            
            var documentDeliveryType = await context.DocumentDeliveryTypes
                .Where(x => x.Id == documentDeliveryTypeId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("DocumentDeliveryType not found");
            
            documentDeliveryType.Names = dto.Names;

            context.DocumentDeliveryTypes.Update(documentDeliveryType);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> DeleteDocumentDeliveryTypeAsync(int documentDeliveryTypeId, CancellationToken ct)
    {
        try
        {
            if (documentDeliveryTypeId == 0) throw new Exception("documentDeliveryTypeId is required");
            
            var documentDeliveryType = await context.DocumentDeliveryTypes
                .Where(x => x.Id == documentDeliveryTypeId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("DocumentDeliveryType not found");

            context.DocumentDeliveryTypes.Remove(documentDeliveryType);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    
    // ControlTypes
    public async Task<Result<IEnumerable<ControlTypeDto>, Exception>> GetAllControlTypesAsync()
    {
        try
        {
            var controlTypes = GetAllControlTypes(context) ?? throw new Exception("ControlTypes not found");
            return await controlTypes.ToListAsync();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<ControlTypeDto, Exception>> CreateControlTypeAsync(CreateControlTypeDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Name.Length == 0) throw new Exception("Name is required");

            var controlType = new ControlType
            {
                Name = dto.Name
            };

            await context.ControlTypes.AddAsync(controlType, ct);
            await context.SaveChangesAsync(ct);
            
            return controlType.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> UpdateControlTypeAsync(int controlTypeId, UpdateControlTypeDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Name.Length == 0) throw new Exception("Name is required");
            
            var controlType = await context.ControlTypes
                .Where(x => x.Id == controlTypeId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("ControlType not found");
            
            controlType.Name = dto.Name;

            context.ControlTypes.Update(controlType);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> DeleteControlTypeAsync(int controlTypeId, CancellationToken ct)
    {
        try
        {
            if (controlTypeId == 0) throw new Exception("controlTypeId is required");
            
            var controlType = await context.ControlTypes
                .Where(x => x.Id == controlTypeId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("ControlType not found");

            context.ControlTypes.Remove(controlType);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    
    // MilestoneRequirementTypes
    public async Task<Result<IEnumerable<MilestoneRequirementTypeDto>, Exception>> GetAllMilestoneRequirementTypesAsync()
    {
        try
        {
            var milestoneRequirementTypes = GetAllMilestoneRequirementTypes(context) ?? throw new Exception("MilestoneRequirementTypes not found");
            return await milestoneRequirementTypes.ToListAsync();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<MilestoneRequirementTypeDto, Exception>> CreateMilestoneRequirementTypeAsync(CreateMilestoneRequirementTypeDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");

            var milestoneRequirementType = new MilestoneRequirementType
            {
                Names = dto.Names
            };

            await context.MilestoneRequirementTypes.AddAsync(milestoneRequirementType, ct);
            await context.SaveChangesAsync(ct);
            
            return milestoneRequirementType.ToDto();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> UpdateMilestoneRequirementTypeAsync(int milestoneRequirementTypeId, UpdateMilestoneRequirementTypeDto dto, CancellationToken ct)
    {
        try
        {
            if (dto.Names.Count == 0) throw new Exception("Names is required");
            
            var milestoneRequirementType = await context.MilestoneRequirementTypes
                .Where(x => x.Id == milestoneRequirementTypeId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("MilestoneRequirementType not found");
            
            milestoneRequirementType.Names = dto.Names;

            context.MilestoneRequirementTypes.Update(milestoneRequirementType);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> DeleteMilestoneRequirementTypeAsync(int milestoneRequirementTypeId, CancellationToken ct)
    {
        try
        {
            if (milestoneRequirementTypeId == 0) throw new Exception("milestoneRequirementTypeId is required");
            
            var milestoneRequirementType = await context.MilestoneRequirementTypes
                .Where(x => x.Id == milestoneRequirementTypeId)
                .FirstOrDefaultAsync(ct) ?? throw new Exception("MilestoneRequirementType not found");

            context.MilestoneRequirementTypes.Remove(milestoneRequirementType);
            await context.SaveChangesAsync(ct);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public string InitializeTempFileDirectory()
    {
        var tempPath = Path.Combine(configuration.GetValue<string>("DocumentTempFolder")!, CreateTempFileDirectory());
        return tempPath;
    }

    public void DeleteTempFileDirectory(string tempPath)
    {
        ClearFolder(tempPath);
    }

    private void ClearFolder(string FolderName)
    {
        DirectoryInfo dir = new DirectoryInfo(FolderName);

        foreach(FileInfo fi in dir.GetFiles())
        {
            fi.Delete();
        }

        foreach (DirectoryInfo di in dir.GetDirectories())
        {
            ClearFolder(di.FullName);
            di.Delete();
        }

        dir.Delete();
    }

    private string CreateTempFileDirectory()
    {
        var di = new DirectoryInfo(configuration.GetValue<string>("DocumentTempFolder")!);

        var tmpDirName = "";
        if (di.Exists)
        {
            tmpDirName = GetTempFileDirectoryName();
            di.CreateSubdirectory(tmpDirName);
        }

        return tmpDirName;
    }

    private string GetTempFileDirectoryName()
    {
        var s = "tmp";

        s += DateTime.Now.Year.ToString();
        s += DateTime.Now.Month.ToString();
        s += DateTime.Now.Day.ToString();
        s += DateTime.Now.Hour.ToString();
        s += DateTime.Now.Minute.ToString();
        s += DateTime.Now.Second.ToString();

        var di = new DirectoryInfo(Path.Combine(configuration.GetValue<string>("DocumentTempFolder")!,s));

        while (di.Exists)
        {
            s = "tmp";
            s += DateTime.Now.Year.ToString();
            s += DateTime.Now.Month.ToString();
            s += DateTime.Now.Day.ToString();
            s += DateTime.Now.Hour.ToString();
            s += DateTime.Now.Minute.ToString();
            s += DateTime.Now.Second.ToString();

            di = new DirectoryInfo(Path.Combine(configuration.GetValue<string>("DocumentTempFolder")!,s));
        }

        return s;
    }
    
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<ApplicationBudgetTypeDto>> GetAllApplicationBudgetTypes = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.ApplicationBudgetTypes
                .AsNoTracking()
                .TagWith("GetAllApplicationBudgetTypes")
                .OrderBy(x => x.Id)
                .Select(x => new ApplicationBudgetTypeDto() { Id = x.Id, Names = x.Names }));
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<DocumentTypeDto>> GetAllDocumentTypes = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.DocumentTypes
                .AsNoTracking()
                .TagWith("GetAllDocumentTypes")
                .OrderBy(x => x.Id)
                .Select(x => new DocumentTypeDto() { Id = x.Id, Names = x.Names }));
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<DocumentDeliveryTypeDto>> GetAllDocumentDeliveryTypes = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.DocumentDeliveryTypes
                .AsNoTracking()
                .TagWith("GetAllDocumentDeliveryTypes")
                .OrderBy(x => x.Id)
                .Select(x => new DocumentDeliveryTypeDto() { Id = x.Id, Names = x.Names }));
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<ControlTypeDto>> GetAllControlTypes = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.ControlTypes
                .AsNoTracking()
                .TagWith("GetAllControlTypes")
                .OrderBy(x => x.Id)
                .Select(x => new ControlTypeDto() { Id = x.Id, Name = x.Name }));
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<MilestoneRequirementTypeDto>> GetAllMilestoneRequirementTypes = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.MilestoneRequirementTypes
                .AsNoTracking()
                .TagWith("GetAllMilestoneRequirementTypes")
                .OrderBy(x => x.Id)
                .Select(x => new MilestoneRequirementTypeDto() { Id = x.Id, Names = x.Names }));
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<EventTypeDto>> GetAllEventTypes = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.EventTypes
                .AsNoTracking()
                .TagWith("GetAllEventTypes")
                .OrderBy(x => x.Id)
                .Select(x => new EventTypeDto() { Id = x.Id, Names = x.Names }));
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<ActionTypeDto>> GetAllActionTypes = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.ActionTypes
                .AsNoTracking()
                .TagWith("GetAllActionTypes")
                .OrderBy(x => x.Id)
                .Select(x => new ActionTypeDto() { Id = x.Id, Names = x.Names }));
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<SystemMessageDestinationDto>> GetAllSystemMessageDestinationTypes = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.SystemMessageDestinations
                .AsNoTracking()
                .TagWith("GetAllSystemMessageDestinationTypes")
                .OrderBy(x => x.Id)
                .Select(x => new SystemMessageDestinationDto() { Id = x.Id, Names = x.Names }));
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<ReactionTypeDto>> GetAllReactionTypes = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.ReactionTypes
                .AsNoTracking()
                .TagWith("GetAllReactionTypes")
                .OrderBy(x => x.Id)
                .Select(x => new ReactionTypeDto() { Id = x.Id, Names = x.Names }));
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<PhoneNumberTypeDto>> GetAllPhoneNumberTypes = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.PhoneNumberTypes
                .AsNoTracking()
                .TagWith("GetAllPhoneNumberTypes")
                .OrderBy(x => x.Id)
                .Select(x => new PhoneNumberTypeDto() { Id = x.Id, Names = x.Names }));
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<MessageTypeDto>> GetAllMessageTypes = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.MessageTypes
                .AsNoTracking()
                .TagWith("GetAllMessageTypes")
                .OrderBy(x => x.Id)
                .Select(x => new MessageTypeDto() { Id = x.Id, Names = x.Names }));
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<ClaimTypeDto>> GetAllClaimTypes = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.ClaimTypes
                .AsNoTracking()
                .TagWith("GetAllClaimTypes")
                .OrderBy(x => x.Id)
                .Select(x => new ClaimTypeDto() { Id = x.Id, Names = x.Names, Tag = x.Tag}));
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<GenderDto>> GetAllGenders = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.Genders
                .AsNoTracking()
                .TagWith("GetAllGenders")
                .OrderBy(x => x.Id)
                .Select(x => new GenderDto() { Id = x.Id, Names = x.Names }));
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<SectionDto>> GetAllSections = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.Sections
                .AsNoTracking()
                .TagWith("GetAllSections")
                .OrderBy(x => x.Id)
                .Select(x => new SectionDto { Id = x.Id, Names = x.Names, Order = x.Order, Enabled = x.Enabled}));
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<StatusDto>> GetAllStatuses = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.Statuses
                .AsNoTracking()
                .TagWith("GetAllStatuses")
                .OrderBy(x => x.Id)
                .Select(x => new StatusDto { Id = x.Id, Names = x.Names }));
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<StatusDto>> GetAllProjectStatuses = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.Statuses
                .AsNoTracking()
                .TagWith("GetAllProjectStatus")
                .Where(x => _allProjectStatuses.Contains(x.Id))
                .OrderBy(x => x.Id)
                .Select(x => new StatusDto { Id = x.Id, Names = x.Names }));
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<StatusDto>> GetSupportedProjectStatuses = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.Statuses
                .AsNoTracking()
                .TagWith("GetAllProjectStatus")
                .Where(x => _supportedProjectStatuses.Contains(x.Id))
                .OrderBy(x => x.Id)
                .Select(x => new StatusDto { Id = x.Id, Names = x.Names }));
    
    private static readonly Func<ApplicationDbContext, IAsyncEnumerable<StatusDto>> GetNotActiveProjectStatuses = 
        EF.CompileAsyncQuery((ApplicationDbContext context) => 
            context.Statuses
                .AsNoTracking()
                .TagWith("GetAllProjectStatus")
                .Where(x => _notActiveProjectStatuses.Contains(x.Id))
                .OrderBy(x => x.Id)
                .Select(x => new StatusDto { Id = x.Id, Names = x.Names }));
}
