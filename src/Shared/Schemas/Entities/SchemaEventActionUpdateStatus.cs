using System;

namespace Shared.Schemas.Entities;

public class SchemaEventActionUpdateStatus(
    int identifier, 
    int changeStatusToId, 
    DateTime? executionDate = null) 
    : SchemaEventAction(
        identifier, 
        4,
        changeStatusToId,
        executionDate ?? DateTime.MinValue);