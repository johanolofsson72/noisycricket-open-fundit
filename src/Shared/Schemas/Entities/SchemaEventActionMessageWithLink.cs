
using System;

namespace Shared.Schemas.Entities;

public class SchemaEventActionMessageWithLink(
    int identifier, 
    int receiverClaimTypeId, 
    int systemDestination, 
    string systemMessage,
    string documentLink,
    DateTime? executionDate = null) 
    : SchemaEventAction(
        identifier, 
        2,
        receiverClaimTypeId,
        systemDestination,
        systemMessage,
        documentLink,
        executionDate ?? DateTime.MinValue);