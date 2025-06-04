using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Applications.Entities;
using Shared.Schemas.DTOs;

namespace Shared.Schemas.Entities;

public class SchemaEventAction
{
    public SchemaEventAction(){}
    
    public SchemaEventAction(
        int identifier, 
        int bindToControlId)
    {
        Id = identifier;
        SchemaEventActionIdentifier = identifier;
        BindToControlId = bindToControlId;
    }
    
    public SchemaEventAction(
        int identifier, 
        int actionTypeId,
        int receiverClaimTypeId, 
        int systemMessageDestinationId, 
        string systemMessage,
        DateTime? executionDate = null,
        int? reactionDescriptionId = null)
    {
        Id = identifier;
        SchemaEventActionIdentifier = identifier;
        ActionTypeId = actionTypeId;
        ReceiverClaimTypeId = receiverClaimTypeId;
        SystemMessageDestinationId = systemMessageDestinationId;
        SystemMessage = systemMessage;
        ExecutionDate = executionDate ?? DateTime.MinValue;
        ReactionDescriptionId = reactionDescriptionId ?? 0;
    }
    
    public SchemaEventAction(
        int identifier, 
        int actionTypeId,
        int receiverClaimTypeId, 
        int systemMessageDestinationId, 
        string systemMessage,
        string documentLink,
        DateTime? executionDate = null)
    {
        Id = identifier;
        SchemaEventActionIdentifier = identifier;
        ActionTypeId = actionTypeId;
        ReceiverClaimTypeId = receiverClaimTypeId;
        SystemMessageDestinationId = systemMessageDestinationId;
        SystemMessage = systemMessage;
        DocumentLink = documentLink;
        ExecutionDate = executionDate ?? DateTime.MinValue;
    }
    
    public SchemaEventAction(
        int identifier, 
        int actionTypeId,
        int deleteEventId,
        int deleteActionId,
        DateTime? executionDate = null)
    {
        Id = identifier;
        SchemaEventActionIdentifier = identifier;
        ActionTypeId = actionTypeId;
        DeleteEventId = deleteEventId;
        DeleteActionId = deleteActionId;
        ExecutionDate = executionDate ?? DateTime.MinValue;
    }
    
    public SchemaEventAction(
        int identifier, 
        int actionTypeId,
        int changeStatusToId, 
        DateTime? executionDate = null)
    {
        Id = identifier;
        SchemaEventActionIdentifier = identifier;
        ActionTypeId = actionTypeId;
        ChangeStatusToId = changeStatusToId;
        ExecutionDate = executionDate ?? DateTime.MinValue;
    }
    
    public SchemaEventAction(
        int identifier, 
        int actionTypeId,
        int receiverClaimTypeId, 
        string emailMessageBody, 
        DateTime? executionDate = null,
        int? reactionDescriptionId = null)
    {
        Id = identifier;
        SchemaEventActionIdentifier = identifier;
        ActionTypeId = actionTypeId;
        ReceiverClaimTypeId = receiverClaimTypeId;
        EmailMessageBody = emailMessageBody;
        ExecutionDate = executionDate ?? DateTime.MinValue;
        ReactionDescriptionId = reactionDescriptionId ?? 0;
    }
    
    public SchemaEventAction(
        int identifier, 
        int actionTypeId,
        int receiverClaimTypeId, 
        string emailMessageBody, 
        string documentLink,
        DateTime? executionDate = null)
    {
        Id = identifier;
        ActionTypeId = actionTypeId;
        SchemaEventActionIdentifier = identifier;
        ReceiverClaimTypeId = receiverClaimTypeId;
        EmailMessageBody = emailMessageBody;
        DocumentLink = documentLink;
        ExecutionDate = executionDate ?? DateTime.MinValue;
    }

    public int Id { get; set; } = 0;
    public int SchemaEventIdentifier { get; set; } = 0;
    public int SchemaEventActionIdentifier { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public int ActionTypeId { get; set; } = 0;
    public DateTime ExecutionDate { get; set; } = DateTime.MinValue;
    public int ReceiverClaimTypeId { get; set; } = 0;
    public string SystemMessage { get; set; } = string.Empty;
    public int SystemMessageDestinationId { get; set; } = 0;
    public List<int> SystemMessagesCreated { get; set; } = new();
    public string DocumentLink { get; set; } = string.Empty;
    public string EmailMessageBody { get; set; } = string.Empty;
    public int ReactionDescriptionId { get; set; } = 0;
    public int SystemMessageIdToDelete { get; set; } = 0;
    public int DeleteEventId { get; set; } = 0;
    public int DeleteActionId { get; set; } = 0;
    public int BindToControlId { get; set; } = 0;
    public string EventActionCombo { get; set; } = string.Empty;
    public int ChangeStatusToId { get; set; } = 0;
    public virtual SchemaEvent Event { get; set; } = new SchemaEvent();
}

public static class SchemaEventActionExtensions
{
    public static List<SchemaEventActionDto> ToSchemaEventDto(this List<SchemaEventAction> entities)
    {
        return entities.Select(x => x.ToSchemaEventActionDto()).ToList();
    }
    
    public static List<SchemaEventAction> ToListSchemaEventAction(this List<SchemaEventActionDto> dtos)
    {
        return dtos.Select(x => x.ToSchemaEventAction()).ToList();
    }
    
    public static List<ApplicationEventAction> ToApplicationEventAction(this List<SchemaEventAction> entities)
    {
        return entities.Select(x => x.ToApplicationEventAction()).ToList();
    }
    
    public static List<SchemaEventAction> ToSchemaEventAction(this List<ApplicationEventAction> dtos)
    {
        return dtos.Select(x => x.ToSchemaEventAction()).ToList();
    }
    
    public static ApplicationEventAction ToApplicationEventAction(this SchemaEventAction entity)
    {
        return new ApplicationEventAction()
        {
            Id = entity.Id,
            ApplicationEventIdentifier = entity.SchemaEventIdentifier,
            ApplicationEventActionIdentifier = entity.SchemaEventActionIdentifier,
            ActionTypeId = entity.ActionTypeId,
            ExecutionDate = entity.ExecutionDate,
            ReceiverClaimTypeId = entity.ReceiverClaimTypeId,
            SystemMessage = entity.SystemMessage,
            SystemMessageDestinationId = entity.SystemMessageDestinationId,
            DocumentLink = entity.DocumentLink,
            EmailMessageBody = entity.EmailMessageBody,
            ReactionDescriptionId = entity.ReactionDescriptionId,
            SystemMessageIdToDelete = entity.SystemMessageIdToDelete,
            DeleteEventId = entity.DeleteEventId,
            DeleteActionId = entity.DeleteActionId,
            EventActionCombo = entity.EventActionCombo,
            ChangeStatusToId = entity.ChangeStatusToId
        };
    }
    
    public static SchemaEventAction ToSchemaEventAction(this ApplicationEventAction dto)
    {
        return new SchemaEventAction()
        {
            Id = dto.Id,
            SchemaEventIdentifier = dto.ApplicationEventIdentifier,
            SchemaEventActionIdentifier = dto.ApplicationEventActionIdentifier,
            ActionTypeId = dto.ActionTypeId,
            ExecutionDate = dto.ExecutionDate,
            ReceiverClaimTypeId = dto.ReceiverClaimTypeId,
            SystemMessage = dto.SystemMessage,
            SystemMessageDestinationId = dto.SystemMessageDestinationId,
            DocumentLink = dto.DocumentLink,
            EmailMessageBody = dto.EmailMessageBody,
            ReactionDescriptionId = dto.ReactionDescriptionId,
            SystemMessageIdToDelete = dto.SystemMessageIdToDelete,
            DeleteEventId = dto.DeleteEventId,
            DeleteActionId = dto.DeleteActionId,
            EventActionCombo = dto.EventActionCombo,
            ChangeStatusToId = dto.ChangeStatusToId
        };
    }
    public static List<SchemaEventActionDto> ToSchemaEventActionDto(this List<SchemaEventAction> entities)
    {
        return entities.Select(x => x.ToSchemaEventActionDto()).ToList();
    }
    
    public static List<SchemaEventAction> ToSchemaEventAction(this List<SchemaEventActionDto> dtos)
    {
        return dtos.Select(x => x.ToSchemaEventAction()).ToList();
    }
    
    public static SchemaEventActionDto ToSchemaEventActionDto(this SchemaEventAction entity)
    {
        return new SchemaEventActionDto()
        {
            Id = entity.Id,
            SchemaEventIdentifier = entity.SchemaEventIdentifier,
            SchemaEventActionIdentifier = entity.SchemaEventActionIdentifier,
            ActionTypeId = entity.ActionTypeId,
            ExecutionDate = entity.ExecutionDate,
            ReceiverClaimTypeId = entity.ReceiverClaimTypeId,
            SystemMessage = entity.SystemMessage,
            SystemMessageDestinationId = entity.SystemMessageDestinationId,
            DocumentLink = entity.DocumentLink,
            EmailMessageBody = entity.EmailMessageBody,
            ReactionDescriptionId = entity.ReactionDescriptionId,
            SystemMessageIdToDelete = entity.SystemMessageIdToDelete,
            DeleteEventId = entity.DeleteEventId,
            DeleteActionId = entity.DeleteActionId,
            EventActionCombo = entity.EventActionCombo,
            ChangeStatusToId = entity.ChangeStatusToId
        };
    }
    
    public static SchemaEventAction ToSchemaEventAction(this SchemaEventActionDto dto)
    {
        return new SchemaEventAction()
        {
            Id = dto.Id,
            SchemaEventIdentifier = dto.SchemaEventIdentifier,
            SchemaEventActionIdentifier = dto.SchemaEventActionIdentifier,
            ActionTypeId = dto.ActionTypeId,
            ExecutionDate = dto.ExecutionDate,
            ReceiverClaimTypeId= dto.ReceiverClaimTypeId,
            SystemMessage = dto.SystemMessage,
            SystemMessageDestinationId = dto.SystemMessageDestinationId,
            DocumentLink = dto.DocumentLink,
            EmailMessageBody = dto.EmailMessageBody,
            ReactionDescriptionId = dto.ReactionDescriptionId,
            SystemMessageIdToDelete = dto.SystemMessageIdToDelete,
            DeleteEventId = dto.DeleteEventId,
            DeleteActionId = dto.DeleteActionId,
            EventActionCombo = dto.EventActionCombo,
            ChangeStatusToId = dto.ChangeStatusToId
        };
    }
    
    public static SchemaEventActionDto ToSchemaEventActionDto(this ApplicationEventAction entity)
    {
        return new SchemaEventActionDto()
        {
            SchemaEventIdentifier = entity.ApplicationEventIdentifier,
            SchemaEventActionIdentifier = entity.ApplicationEventActionIdentifier,
            ActionTypeId = entity.ActionTypeId,
            ExecutionDate = entity.ExecutionDate,
            ReceiverClaimTypeId = entity.ReceiverClaimTypeId,
            SystemMessage = entity.SystemMessage,
            SystemMessageDestinationId = entity.SystemMessageDestinationId,
            DocumentLink = entity.DocumentLink,
            EmailMessageBody = entity.EmailMessageBody,
            ReactionDescriptionId = entity.ReactionDescriptionId,
            SystemMessageIdToDelete = entity.SystemMessageIdToDelete,
            DeleteEventId = entity.DeleteEventId,
            DeleteActionId = entity.DeleteActionId,
            EventActionCombo = entity.EventActionCombo,
            ChangeStatusToId = entity.ChangeStatusToId
        };
    }
    
    public static ApplicationEventAction ToApplicationEventAction(this SchemaEventActionDto dto)
    {
        return new ApplicationEventAction()
        {
            ApplicationEventIdentifier = dto.SchemaEventIdentifier,
            ApplicationEventActionIdentifier = dto.SchemaEventActionIdentifier,
            ActionTypeId = dto.ActionTypeId,
            ExecutionDate = dto.ExecutionDate,
            ReceiverClaimTypeId = dto.ReceiverClaimTypeId,
            SystemMessage = dto.SystemMessage,
            SystemMessageDestinationId = dto.SystemMessageDestinationId,
            DocumentLink = dto.DocumentLink,
            EmailMessageBody = dto.EmailMessageBody,
            ReactionDescriptionId = dto.ReactionDescriptionId,
            SystemMessageIdToDelete = dto.SystemMessageIdToDelete,
            DeleteEventId = dto.DeleteEventId,
            DeleteActionId = dto.DeleteActionId,
            EventActionCombo = dto.EventActionCombo,
            ChangeStatusToId = dto.ChangeStatusToId
        };
    }
    
    public static List<SchemaEventActionDto> ToSchemaEventActionDto(this List<ApplicationEventAction> entities)
    {
        return entities.Select(x => x.ToSchemaEventActionDto()).ToList();
    }
    
    public static List<ApplicationEventAction> ToApplicationEventAction(this List<SchemaEventActionDto> dtos)
    {
        return dtos.Select(x => x.ToApplicationEventAction()).ToList();
    }
}
