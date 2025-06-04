
using System;

namespace Shared.Schemas.Entities;

public class SchemaEventActionEmailWithLink(
    int identifier, 
    int receiverClaimTypeId, 
    string emailMessageBody, 
    string documentLink,
    DateTime? executionDate = null) 
    : SchemaEventAction(
        identifier, 
        3, 
        receiverClaimTypeId,
        emailMessageBody,
        documentLink,
        executionDate ?? DateTime.MinValue);