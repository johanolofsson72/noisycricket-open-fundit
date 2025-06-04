using System;
using System.Collections.Generic;
using Shared.Applications.DTOs;

namespace Shared.Applications.Entities;

public class ApplicationEventAction
{
    
    public ApplicationEventAction(){}
    
    public ApplicationEventAction(
        int identifier, 
        int actionTypeId,
        int receiverClaimTypeId, 
        int systemMessageDestinationId, 
        string systemMessage,
        DateTime? executionDate = null,
        int? reactionDescriptionId = null)
    {
        Id = identifier;
        ApplicationEventActionIdentifier = identifier;
        ActionTypeId = actionTypeId;
        ReceiverClaimTypeId = receiverClaimTypeId;
        SystemMessageDestinationId = systemMessageDestinationId;
        SystemMessage = systemMessage;
        ExecutionDate = executionDate ?? DateTime.MinValue;
        ReactionDescriptionId = reactionDescriptionId ?? 0;
    }
    
    public ApplicationEventAction(
        int identifier, 
        int actionTypeId,
        int receiverClaimTypeId, 
        int systemMessageDestinationId, 
        string systemMessage,
        string documentLink,
        DateTime? executionDate = null)
    {
        Id = identifier;
        ApplicationEventActionIdentifier = identifier;
        ActionTypeId = actionTypeId;
        ReceiverClaimTypeId = receiverClaimTypeId;
        SystemMessageDestinationId = systemMessageDestinationId;
        SystemMessage = systemMessage;
        DocumentLink = documentLink;
        ExecutionDate = executionDate ?? DateTime.MinValue;
    }
    
    public ApplicationEventAction(
        int identifier, 
        int actionTypeId,
        int deleteEventId,
        int deleteActionId,
        DateTime? executionDate = null)
    {
        Id = identifier;
        ApplicationEventActionIdentifier = identifier;
        ActionTypeId = actionTypeId;
        DeleteEventId = deleteEventId;
        DeleteActionId = deleteActionId;
        ExecutionDate = executionDate ?? DateTime.MinValue;
    }
    
    public ApplicationEventAction(
        int identifier, 
        int actionTypeId,
        int changeStatusToId, 
        DateTime? executionDate = null)
    {
        Id = identifier;
        ApplicationEventActionIdentifier = identifier;
        ActionTypeId = actionTypeId;
        ChangeStatusToId = changeStatusToId;
        ExecutionDate = executionDate ?? DateTime.MinValue;
    }
    
    public ApplicationEventAction(
        int identifier, 
        int actionTypeId,
        int receiverClaimTypeId, 
        string emailMessageBody, 
        DateTime? executionDate = null,
        int? reactionDescriptionId = null)
    {
        Id = identifier;
        ApplicationEventActionIdentifier = identifier;
        ActionTypeId = actionTypeId;
        ReceiverClaimTypeId = receiverClaimTypeId;
        EmailMessageBody = emailMessageBody;
        ExecutionDate = executionDate ?? DateTime.MinValue;
        ReactionDescriptionId = reactionDescriptionId ?? 0;
    }
    
    public ApplicationEventAction(
        int identifier, 
        int actionTypeId,
        int receiverClaimTypeId, 
        string emailMessageBody, 
        string documentLink,
        DateTime? executionDate = null)
    {
        Id = identifier;
        ApplicationEventActionIdentifier = identifier;
        ActionTypeId = actionTypeId;
        ReceiverClaimTypeId = receiverClaimTypeId;
        EmailMessageBody = emailMessageBody;
        DocumentLink = documentLink;
        ExecutionDate = executionDate ?? DateTime.MinValue;
    }
    public int Id { get; set; } = 0;
    public int ApplicationEventIdentifier { get; set; } = 0;
    public int ApplicationEventActionIdentifier { get; set; } = 0;
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
    public string EventActionCombo { get; set; } = string.Empty;
    public int ChangeStatusToId { get; set; } = 0;
    public virtual ApplicationEvent Event { get; set; } = new ApplicationEvent();
}



public static class ApplicationEventActionExtensions
{
    public static ApplicationEventActionDto ToDto(this ApplicationEventAction entity) =>
        new ApplicationEventActionDto()
        {
            Id = entity.Id,
            ApplicationEventActionIdentifier = entity.ApplicationEventActionIdentifier,
            StatusId = entity.StatusId,
            ActionTypeId = entity.ActionTypeId,
            ExecutionDate = entity.ExecutionDate,
            ReceiverClaimTypeId = entity.ReceiverClaimTypeId,
            SystemMessage = entity.SystemMessage,
            SystemMessageDestinationId = entity.SystemMessageDestinationId,
            SystemMessagesCreated = entity.SystemMessagesCreated,
            DocumentLink = entity.DocumentLink,
            EmailMessageBody = entity.EmailMessageBody,
            ReactionDescriptionId = entity.ReactionDescriptionId,
            SystemMessageIdToDelete = entity.SystemMessageIdToDelete,
            DeleteEventId = entity.DeleteEventId,
            DeleteActionId = entity.DeleteActionId,
            EventActionCombo = entity.EventActionCombo,
            ChangeStatusToId= entity.ChangeStatusToId
        };
    
    public static ApplicationEventAction ToEntity(this ApplicationEventActionDto dto) =>
        new ApplicationEventAction()
        {
            Id = dto.Id,
            ApplicationEventIdentifier = dto.ApplicationEventIdentifier,
            ApplicationEventActionIdentifier = dto.ApplicationEventActionIdentifier,
            StatusId = dto.StatusId,
            ActionTypeId = dto.ActionTypeId,
            ExecutionDate = dto.ExecutionDate,
            ReceiverClaimTypeId = dto.ReceiverClaimTypeId,
            SystemMessage = dto.SystemMessage,
            SystemMessageDestinationId = dto.SystemMessageDestinationId,
            SystemMessagesCreated = dto.SystemMessagesCreated,
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