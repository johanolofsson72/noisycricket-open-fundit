
using System;
using System.Collections.Generic;

namespace Shared.Schemas.DTOs;

public class SchemaEventActionDto
{
    public SchemaEventActionDto(){}
    
    public SchemaEventActionDto(
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
        SchemaEventActionIdentifier = identifier;
        ReceiverClaimTypeId = receiverClaimTypeId;
        SystemMessageDestinationId = systemMessageDestinationId;
        SystemMessage = systemMessage;
        ExecutionDate = executionDate ?? DateTime.MinValue;
        ReactionDescriptionId = reactionDescriptionId ?? 0;
    }
    
    public SchemaEventActionDto(
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
        SchemaEventActionIdentifier = identifier;
        ReceiverClaimTypeId = receiverClaimTypeId;
        SystemMessageDestinationId = systemMessageDestinationId;
        SystemMessage = systemMessage;
        DocumentLink = documentLink;
        ExecutionDate = executionDate ?? DateTime.MinValue;
    }
    
    public SchemaEventActionDto(
        int identifier, 
        int actionTypeId,
        int deleteEventId,
        int deleteActionId,
        DateTime? executionDate = null)
    {
        Id = identifier;
        SchemaEventActionIdentifier = identifier;
        ActionTypeId = actionTypeId;
        SchemaEventActionIdentifier = identifier;
        DeleteEventId = deleteEventId;
        DeleteActionId = deleteActionId;
        ExecutionDate = executionDate ?? DateTime.MinValue;
    }
    
    public SchemaEventActionDto(
        int identifier, 
        int actionTypeId,
        int changeStatusToId, 
        DateTime? executionDate = null)
    {
        Id = identifier;
        SchemaEventActionIdentifier = identifier;
        ActionTypeId = actionTypeId;
        SchemaEventActionIdentifier = identifier;
        ChangeStatusToId = changeStatusToId;
        ExecutionDate = executionDate ?? DateTime.MinValue;
    }
    
    public SchemaEventActionDto(
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
        SchemaEventActionIdentifier = identifier;
        ReceiverClaimTypeId = receiverClaimTypeId;
        EmailMessageBody = emailMessageBody;
        ExecutionDate = executionDate ?? DateTime.MinValue;
        ReactionDescriptionId = reactionDescriptionId ?? 0;
    }
    
    public SchemaEventActionDto(
        int identifier, 
        int actionTypeId,
        int receiverClaimTypeId, 
        string emailMessageBody, 
        string documentLink,
        DateTime? executionDate = null)
    {
        Id = identifier;
        SchemaEventActionIdentifier = identifier;
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
    public string EventActionCombo { get; set; } = string.Empty;
    public int ChangeStatusToId { get; set; } = 0;
}