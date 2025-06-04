using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Shared.Data.DbContext;
using Shared.Events.Entities;
using Shared.Global.Services;

namespace Shared.Events.Services;

public class EventService(IDbContextFactory<ApplicationDbContext> factory, IEasyCachingProvider cache, MessageService messageService, IEmailSender emailSender, SharedService sharedService)
{
    public async Task<Result<ApplicationEventDto, Exception>> CheckNextEvent(int applicationId, CancellationToken ct, int? preferredSelection = null)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");

            // Hämta applikationen utan Include
            await using var context = await factory.CreateDbContextAsync(ct);
            var application = await context.Applications
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == applicationId, ct);
            if (application is null) throw new Exception($"Application {applicationId} not found");

            // Hämta nästa event att trigga
            var ev = new ApplicationEvent();
            var nextEventResult = await NextEvent(applicationId, ct);
            if (!nextEventResult.IsOk) throw nextEventResult.Error;
            
            // Kontrollera om föregående event, om det finns, är den sista i kedjan
            var nextEvent = application.Events
                .FirstOrDefault(e => e.ApplicationEventIdentifier == nextEventResult.Value.ApplicationEventIdentifier);
            if (!nextEvent!.IsFirstInChain)
            {
                var previousEvent = application.Events
                    .FirstOrDefault(e => e.ApplicationEventIdentifier == nextEvent.DependOnEventId);

                if (previousEvent != null) ev = NextEventBySelection(application, previousEvent, nextEvent, preferredSelection);
            }
            else
            {
                ev = application.Events.FirstOrDefault(x =>
                    x.ApplicationEventIdentifier == nextEventResult.Value.ApplicationEventIdentifier);
            }
            
            if (ev == null) throw new Exception($"Event not found for Application {applicationId}");
            
            var evToReturn = ev.ToDto();
            return evToReturn; 
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<TriggerEvent, Exception>> TriggerNextEventById(int applicationId, int eventId, CancellationToken ct, int? preferredSelection = null)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            if (eventId == 0) throw new Exception("eventId is required");

            // Hämta applikationen utan Include
            await using var context = await factory.CreateDbContextAsync(ct);
            var application = await context.Applications
                .AsTracking()
                .FirstOrDefaultAsync(x => x.Id == applicationId, ct);
            if (application is null) throw new Exception($"Application {applicationId} not found");

            // Hämta nästa event att trigga
            var ev = application.Events.FirstOrDefault(x => x.ApplicationEventIdentifier == eventId);
            if (ev == null) throw new Exception($"Event not found for Application {applicationId}");

            if (eventId > 1)
            {
                // Hämta förra eventet
                var previousEvent = application.Events.FirstOrDefault(x => x.ApplicationEventIdentifier == ev.DependOnEventId);
                if (previousEvent != null)
                {
                    Console.WriteLine($"The previous event is: {previousEvent.Id} - {previousEvent.Description} deleted = {previousEvent.StatusId == 16}");
                    Console.WriteLine($"This event will be triggered: {ev.Id} - {ev.Description} deleted = {ev.StatusId == 16}");
                    if (previousEvent.StatusId != 16) return new TriggerEvent();
                    ev = NextEventBySelection(application, previousEvent, ev, preferredSelection);
                }
            }
            
            
            var triggerEvent = await TriggerNextEventActions(applicationId, ct, application, ev);

            // Markera eventuella alternativa event som bearbetade
            if (preferredSelection > 0)
            {
                MarkAlternativeEventsAsDeleted(application, ev.ApplicationEventIdentifier, ev.DependOnEventId, preferredSelection, false);
            }
            
            // Markera eventet som bearbetat
            MarkEventAsDeleted(ev);
            
            application.NewEventCounter++;
                
            await context.SaveChangesAsync(ct);
            
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Applications.ToDescriptionString(), ct);        
            await cache.RemoveByPrefixAsync(CacheKeyPrefix.Projects.ToDescriptionString(), ct);
            
            return triggerEvent; 
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> TriggerNextEvent(int applicationId, CancellationToken ct, int? preferredSelection = null, bool disableDeleteOfAlternativeChain = false)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");

            // Hämta applikationen utan Include
            await using var context = await factory.CreateDbContextAsync(ct);
            var application = await context.Applications
                .AsTracking()
                .FirstOrDefaultAsync(x => x.Id == applicationId, ct);
            if (application is null) throw new Exception($"Application {applicationId} not found");

            // Hämta nästa event att trigga
            var ev = new ApplicationEvent();
            var nextEventResult = await NextEvent(applicationId, ct);
            if (!nextEventResult.IsOk) throw nextEventResult.Error;
            
            var nextEvent = application.Events
                .FirstOrDefault(e => e.ApplicationEventIdentifier == nextEventResult.Value.ApplicationEventIdentifier);
            
            // Kontrollera om föregående event, om det finns, är den sista i kedjan
            if (!nextEvent!.IsFirstInChain)
            {
                var previousEvent = application.Events
                    .FirstOrDefault(e => e.ApplicationEventIdentifier == nextEvent.DependOnEventId);

                if (previousEvent != null) ev = NextEventBySelection(application, previousEvent, nextEvent, preferredSelection);
            }
            else
            {
                ev = application.Events.FirstOrDefault(x =>
                    x.ApplicationEventIdentifier == nextEventResult.Value.ApplicationEventIdentifier);
            }
            
            if (ev == null) throw new Exception($"Event not found for Application {applicationId}");

            var result = await TriggerNextEventActions(applicationId, ct, application, ev);

            // Markera eventuella alternativa event som bearbetade
            if (preferredSelection > 0)
            {
                MarkAlternativeEventsAsDeleted(application, ev.ApplicationEventIdentifier, ev.DependOnEventId, preferredSelection, disableDeleteOfAlternativeChain);
            }

            // Markera eventet som bearbetat
            MarkEventAsDeleted(ev);
            
            application.NewEventCounter++;
                
            await context.SaveChangesAsync(ct);
            
            return true; 
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private async Task<Result<ApplicationEvent, Exception>> NextEvent(int applicationId, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var application = await context.Applications
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == applicationId, ct);

            if (application is null) throw new Exception($"Application {applicationId} not found");

            var allNotDeleted = application.Events
                .Where(x => x.StatusId != 16)
                .OrderBy(x => x.ApplicationEventIdentifier)
                .ToList() ?? throw new Exception($"No upcoming events found for Application {applicationId}");

            var firstNotDeleted = allNotDeleted.FirstOrDefault() ?? throw new Exception($"No ongoing events found for Application {applicationId}");

            var firstInChain = FindFirstNonDeletedInChain(application, firstNotDeleted);

            return firstInChain;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private static ApplicationEvent FindFirstNonDeletedInChain(Application application, ApplicationEvent ev)
    {
        while (ev.DependOnEventId != 0)
        {
            var sourceEvent = application.Events
                .FirstOrDefault(x => x.ApplicationEventIdentifier == ev.DependOnEventId);

            if (sourceEvent == null)
            {
                throw new Exception($"Source event {ev.DependOnEventId} not found for application {application.Id}");
            }

            if (sourceEvent.StatusId != 16)
            {
                ev = sourceEvent;
            }
            else
            {
                break;
            }
        }
        return ev;
    }

    private ApplicationEvent NextEventBySelection(Application application, ApplicationEvent previousEvent, ApplicationEvent nextEvent, int? preferredSelection)
    {
        var potentialEvents = application.Events
            .Where(x => x.DependOnEventId == previousEvent.ApplicationEventIdentifier && x.StatusId != 16)
            .OrderBy(x => x.Order) // För att säkerställa korrekt ordning
            .ToList();

        if (potentialEvents.Count == 0 || !preferredSelection.HasValue) return nextEvent;
        
        if (preferredSelection > 0 && preferredSelection <= potentialEvents.Count)
        {
            return potentialEvents[preferredSelection.Value - 1];
        }
        else
        {
            throw new ArgumentOutOfRangeException($"Preferred selection {preferredSelection} is out of range for Application {application.Id}");
        }
    }

    private async Task<TriggerEvent> TriggerNextEventActions(int applicationId, CancellationToken ct, Application application, ApplicationEvent ev)
    {
        var result = new TriggerEvent { ApplicationStatusId = application.StatusId };
        
        application.Audits.Add(
            new ApplicationAudit()
            {
                Id = application.Audits.Count + 1,
                ApplicationAuditIdentifier = application.Audits.Count + 1,
                Event = "Execute event: " + ev.Description,
                Fields = [application.Id.ToString(), 
                    ev.Id.ToString()],
                Executed = DateTime.UtcNow,
                ExecutedBy = "EventService.TriggerNextEventActions"
            });

        try
        {
            foreach (var action in ev.Actions.Where(x => x.StatusId != 16))
            {
                switch (action.ActionTypeId)
                {
                    case 2:
                    {
                        var response = await ExecuteMessageEventAction(application, ev, action, ct);
                        if (response is { IsOk: true, Value: not null }) result.InternalMessagesCreated.AddRange(response.Value);
                        break;
                    }
                    case 3:
                    {
                        var response = await ExecuteEmailEventAction(application, ev, action, ct);
                        break;
                    }
                    case 4:
                    {
                        var response = await ExecuteStatusUpdateEventAction(application, ev, action, ct);
                        if (!response.Value.isChanged) continue;
                        result.ApplicationStatusId = response.Value.statusId;
                        result.ApplicationStatusIsUpdated = true;
                        break;
                    }
                    case 5:
                    {
                        var response = await ExecuteDeleteMessageEventAction(application, ev, action, ct);
                        result.InternalMessagesDeleted.AddRange(response.Value);
                        break;
                    }
                    case 1:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            result.AmountOfUnprocessedMessages = messageService.AmountOfUnprocessedMessagesAsync(applicationId);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        return result;
    }

    private static void MarkEventAsDeleted(ApplicationEvent ev)
    {
        ev.StatusId = 16;
        foreach (var action in ev.Actions)
        {
            action.StatusId = 16;
        }
        
        Console.WriteLine($@"[EventService] Deleted event {ev.ApplicationEventIdentifier}");
    }

    private static void MarkAlternativeEventsAsDeleted(Application application, int eventId, int dependOnEventId, int? preferredSelection, bool disableDeleteOfAlternativeChain)
    {
        var potentialEvents = application.Events
            .Where(x => x.DependOnEventId == dependOnEventId)
            .Where(x => x.StatusId != 16)
            .Where(x => x.ApplicationEventIdentifier != eventId)
            .OrderBy(x => x.Order) // För att säkerställa korrekt ordning
            .Take(1)
            .ToList();

        foreach (var potentialEvent in potentialEvents)
        {
            MarkChainAsDeleted(application, potentialEvent, disableDeleteOfAlternativeChain);
        }
    }

    private static void MarkChainAsDeleted(Application application, ApplicationEvent ev, bool disableDeleteOfAlternativeChain)
    {
        var potentialEvents = application.Events
            .Where(x => x.DependOnEventId == ev.ApplicationEventIdentifier && x.StatusId != 16)
            .OrderBy(x => x.Order) // För att säkerställa korrekt ordning
            .ToList();

        foreach (var potentialEvent in potentialEvents.Where(x => !disableDeleteOfAlternativeChain))
        {
            MarkChainAsDeleted(application, potentialEvent, false);
        }
        
        MarkEventAsDeleted(ev);
    }
    
    
    
    public async Task<Result<bool, Exception>> TriggerStandaloneEvents(int applicationId, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");

            // Hämta applikationen utan Include
            await using var context = await factory.CreateDbContextAsync(ct);
            var application = await context.Applications
                .AsTracking()
                .FirstOrDefaultAsync(x => x.Id == applicationId, ct);
            if (application is null) throw new Exception($"Application {applicationId} not found");

            // Hämta det första standalone eventet sorterade efter Order
            var standaloneEvents = application.Events
                .Where(x => x.IsStandAlone && x.StatusId != 16)
                .OrderBy(x => x.Order)
                .Take(1)
                .ToList();

            foreach (var ev in standaloneEvents)
            {
                _ = await TriggerNextEventActions(applicationId, ct, application, ev);
                
                // Markera standalone-eventet som bearbetat
                MarkEventAsDeleted(ev);
            }

            application.NewEventCounter++;

            await context.SaveChangesAsync(ct);
            return true; // No exception occurred
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
    
    public async Task<Result<bool, Exception>> TriggerStandAloneEvent(int applicationId, int eventId, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            if (eventId == 0) throw new Exception("eventId is required");

            // Hämta applikationen utan Include
            await using var context = await factory.CreateDbContextAsync(ct);
            var application = await context.Applications
                .AsTracking()
                .FirstOrDefaultAsync(x => x.Id == applicationId, ct);
            if (application is null) throw new Exception($"Application {applicationId} not found");

            // Hämta eventet
            var ev = application.Events
                .FirstOrDefault(x => x.IsStandAlone && x.StatusId != 16 && x.ApplicationEventIdentifier == eventId);
            
            if (ev is null) throw new Exception($"Event {eventId} not found for Application {applicationId}");

            _ = await TriggerNextEventActions(applicationId, ct, application, ev);
                
            // Markera standalone-eventet som bearbetat
            MarkEventAsDeleted(ev);
            
            application.NewEventCounter++;

            await context.SaveChangesAsync(ct);
            return true; // No exception occurred
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<Result<bool, Exception>> TriggerStandAloneEvent(int applicationId, int eventId, int eventToDeleteId, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            if (eventId == 0) throw new Exception("eventId is required");
            if (eventToDeleteId == 0) throw new Exception("eventToDeleteId is required");

            // Hämta applikationen utan Include
            await using var context = await factory.CreateDbContextAsync(ct);
            var application = await context.Applications
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == applicationId, ct);
            if (application is null) throw new Exception($"Application {applicationId} not found");

            // Hämta eventet
            var ev = application.Events
                .FirstOrDefault(x => x.IsStandAlone && x.StatusId != 16 && x.ApplicationEventIdentifier == eventId);
            
            if (ev is null) throw new Exception($"Event {eventId} not found for Application {applicationId}");

            _ = await TriggerNextEventActions(applicationId, ct, application, ev);
                
            // Markera standalone-eventet som bearbetat
            MarkEventAsDeleted(ev);
            
            // Hämta gamla eventet
            ev = application.Events
                .FirstOrDefault(x => x.StatusId != 16 && x.ApplicationEventIdentifier == eventToDeleteId);
            
            if (ev is null) throw new Exception($"Event {eventToDeleteId} not found for Application {applicationId}");
            
            // Markera gamla-eventet som bearbetat
            MarkEventAsDeleted(ev);
            
            application.NewEventCounter++;
            
            await context.SaveChangesAsync(ct);
            return true; // No exception occurred
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    
    
    public async Task<Result<bool, Exception>> IsEventComplete(int applicationId, int eventTypeId, CancellationToken ct)
    {
        try
        {
            if (applicationId == 0) throw new Exception("applicationId is required");
            if (eventTypeId == 0) throw new Exception("eventTypeId is required");

            await using var context = await factory.CreateDbContextAsync(ct);
            var application = await context.Applications
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == applicationId, ct);
            
            if (application is null) throw new Exception($"Application {applicationId} not found");

            var ev = application.Events
                .OrderBy(x => x.Order)
                .FirstOrDefault(x => x.EventTypeId == eventTypeId && x.StatusId == 16);

            return ev is not null;
        }
        catch(Exception ex)
        {
            return ex;
        }
    }

    private async Task<Result<List<int>, Exception>> ExecuteMessageEventAction(Application application, ApplicationEvent ev, ApplicationEventAction action, CancellationToken ct)
    {
        try
        {
            await using var context = await factory.CreateDbContextAsync(ct);
            var project = new Project();
            if (application.ProjectId > 0)
            {
                project = await context.Projects
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == application.ProjectId, ct) ?? new Project();
            }
            var messageReceiverClaimTypeId = action.ReceiverClaimTypeId;
            var messageTypeId = ev.ApplicationEventIdentifier;
            var messageStatusId = 3;
            var messageOrganizationId = application.Organization.ContactIdentifier;
            var messageOrganizationName = application.Organization.Name;
            var messageProjectId = application.ProjectId;
            var messageProjectTitle = project.Title.Count != 0 ? project.Title[0] : application.Title;
            var messageProjectNumber = project.Number;
            var messageApplicationId = application.Id;
            var messageApplicationTitle = application.Title;
            var messageApplicationStatusId = application.StatusId;
            var messageSchemaId = application.SchemaId;
            var messageDocumentLink = action.DocumentLink;
            var messageDocumentTypeId = 1;
            var messageTitle = action.SystemMessage;
            var messageReactionDescriptionId = action.ReactionDescriptionId;
            var messageSystemMessageDestinationId = action.SystemMessageDestinationId;
            var messageExecutionDate = DateTime.UtcNow;
            var messageExpireDate = DateTime.UtcNow.AddMonths(1);
            var outgoing = messageReceiverClaimTypeId == 21;
            
            var claimType = await context.ClaimTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == messageReceiverClaimTypeId, ct) ?? new();
                
            var ids = await context.UserClaims
                .AsNoTracking()
                .Where(x => x.ClaimType == "role")
                .Where(x => x.ClaimValue == claimType.Tag)
                .Select(x => x.UserId)
                .ToListAsync(ct);
            
            if (ids.Count == 0) throw new Exception($"No users found for role {messageReceiverClaimTypeId} when creating actions for application {application.Id}");
            
            var messageReceivers = await context.Users
                .AsNoTracking()
                .Where(x => ids.Contains(x.Id))
                .Select(x => new MessageContactDto()
                {
                    ContactIdentifier = x.Id,
                    Name = x.FullName,
                    Email = x.Email ?? string.Empty
                })
                .ToListAsync(ct);
            
            foreach (var messageReceiver in messageReceivers)
            {
                var createMessage = new CreateMessageDto()
                {
                    Receiver = messageReceiver,
                    EventId = ev.ApplicationEventIdentifier,
                    EventTypeId = ev.EventTypeId,
                    MessageTypeId = 0,
                    Outgoing = outgoing,
                    Incoming = false,
                    StatusId = messageStatusId,
                    OrganizationId = messageOrganizationId,
                    OrganizationName = messageOrganizationName,
                    ProjectId = messageProjectId,
                    ProjectTitle = messageProjectTitle,
                    ProjectNumber = messageProjectNumber,
                    ApplicationId = messageApplicationId,
                    ApplicationTitle = messageApplicationTitle,
                    ApplicationStatusId = messageApplicationStatusId,
                    SchemaId = messageSchemaId,
                    DocumentLink = messageDocumentLink,
                    DocumentTypeId = messageDocumentTypeId,
                    Title = messageTitle,
                    ReactionDescriptionId = messageReactionDescriptionId,
                    SystemMessageDestinationId = messageSystemMessageDestinationId,
                    ExecutionDate = messageExecutionDate,
                    ExpireDate = messageExpireDate
                };
                
                var createMessageResult = await messageService.CreateMessageAsync(createMessage, ct);
                if (!createMessageResult.IsOk) throw createMessageResult.Error;
                action.SystemMessagesCreated.Add(createMessageResult.Value.Id);
                application.Audits.Add(
                    new ApplicationAudit()
                    {
                        Id = application.Audits.Count + 1,
                        ApplicationAuditIdentifier = application.Audits.Count + 1,
                        Event = "Message sent to: " + messageReceiver.Name,
                        Fields = [messageProjectId.ToString(), 
                            messageProjectTitle, 
                            messageProjectNumber, 
                            messageApplicationId.ToString(),
                            messageReceiver.Name, 
                            messageApplicationTitle],
                        Executed = DateTime.UtcNow,
                        ExecutedBy = "EventService.ExecuteMessageEventAction"
                    });
            }
            
            action.StatusId = 16;
            
            return action.SystemMessagesCreated;
        }
        catch(Exception ex)
        {
            return ex;
        }
    }

    private async Task<Result<bool, Exception>> ExecuteEmailEventAction(Application application, ApplicationEvent ev, ApplicationEventAction action, CancellationToken ct)
    {
        try
        {
            // TODO: Implement email sending
            var mailReceiver = application.ProjectManager.Email;
            var mailSubject = "Test email";
            var mailBody = action.EmailMessageBody;
            
            await emailSender.SendEmailAsync(mailReceiver, mailSubject, mailBody);
            
            action.StatusId = 16;
                
            application.Audits.Add(
                new ApplicationAudit()
                {
                    Id = application.Audits.Count + 1,
                    ApplicationAuditIdentifier = application.Audits.Count + 1,
                    Event = "Email sent to: " + mailReceiver,
                    Fields = [application.Id.ToString(), 
                        mailReceiver, 
                        mailSubject, 
                        mailBody],
                    Executed = DateTime.UtcNow,
                    ExecutedBy = "EventService.ExecuteEmailEventAction"
                });
            
            return true;
        }
        catch(Exception ex)
        {
            return ex;
        }
    }

    private async Task<Result<(int statusId, bool isChanged), Exception>> ExecuteStatusUpdateEventAction(Application application, ApplicationEvent ev, ApplicationEventAction action, CancellationToken ct)
    {
        try
        {
            application.StatusId = action.ChangeStatusToId;
            action.StatusId = 16;
            
            await using var context = await factory.CreateDbContextAsync(ct);
            var status = await context.Statuses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == action.ChangeStatusToId, ct);

            if (status is not null)
            {
                application.Audits.Add(
                    new ApplicationAudit()
                    {
                        Id = application.Audits.Count + 1,
                        ApplicationAuditIdentifier = application.Audits.Count + 1,
                        Event = "Status changed to: " + status.Names[sharedService.IndexByCulture()],
                        Fields = [application.Id.ToString(), 
                            application.StatusId.ToString()],
                        Executed = DateTime.UtcNow,
                        ExecutedBy = "EventService.ExecuteStatusUpdateEventAction"
                    });   
            }
            
            return (action.ChangeStatusToId, true);
        }
        catch(Exception ex)
        {
            return ex;
        }
    }

    private async Task<Result<List<int>, Exception>> ExecuteDeleteMessageEventAction(Application application, ApplicationEvent ev, ApplicationEventAction action, CancellationToken ct)
    {
        try
        {
            var eventId = action.DeleteEventId;
            var actionId = action.DeleteActionId;
            var actionToBeDeleted = application.Events
                .FirstOrDefault(x => x.ApplicationEventIdentifier == eventId)
                ?.Actions
                .FirstOrDefault(x => x.ApplicationEventActionIdentifier == actionId);
            var result = new List<int>();

            if (actionToBeDeleted is null) return result;
            
            foreach (var systemMessage in actionToBeDeleted.SystemMessagesCreated)
            {
                var deletedMessage = await messageService.DeleteMessageAsync(systemMessage, ct);
                if (!deletedMessage.IsOk) throw deletedMessage.Error;
                
                result.Add(systemMessage);
                
                application.Audits.Add(
                    new ApplicationAudit()
                    {
                        Id = application.Audits.Count + 1,
                        ApplicationAuditIdentifier = application.Audits.Count + 1,
                        Event = "Delete message: " + systemMessage,
                        Fields = [application.Id.ToString(), 
                            systemMessage.ToString()],
                        Executed = DateTime.UtcNow,
                        ExecutedBy = "EventService.ExecuteDeleteMessageEventAction"
                    });   
            }
            
            action.StatusId = 16;
            
            return result;
        }
        catch(Exception ex)
        {
            return ex;
        }
    }
    
}