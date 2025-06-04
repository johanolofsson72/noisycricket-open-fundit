using System;

namespace Shared.Schemas.Entities;

public class SchemaEventActionMessage(
    int identifier, 
    int receiverClaimTypeId, 
    int systemDestinationId, 
    string systemMessage,
    DateTime? executionDate = null,
    int? reactionDescriptionId = null) 
    : SchemaEventAction(
        identifier, 
        2,
        receiverClaimTypeId,
        systemDestinationId,
        systemMessage,
        executionDate ?? DateTime.MinValue,
        reactionDescriptionId ?? 0);