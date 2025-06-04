
using System;
using System.Collections.Generic;

namespace Shared.Schemas.DTOs;

public class AddSchemaEventActionDto
{
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