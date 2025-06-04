using System;

namespace Shared.Schemas.Entities;

public class SchemaEventActionEmail(
    int identifier, 
    int receiverClaimTypeId, 
    string emailMessageBody, 
    DateTime? executionDate = null,
    int? reactionDescriptionId = null) 
    : SchemaEventAction(
        identifier, 
        3, 
        receiverClaimTypeId,
        emailMessageBody,
        executionDate ?? DateTime.MinValue,
        reactionDescriptionId ?? 0);