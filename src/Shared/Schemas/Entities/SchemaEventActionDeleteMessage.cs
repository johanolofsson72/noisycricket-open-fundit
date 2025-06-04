
using System;

namespace Shared.Schemas.Entities;

public class SchemaEventActionDeleteMessage(
    int identifier, 
    int deleteEventId,
    int deleteActionId,
    DateTime? executionDate = null) 
    : SchemaEventAction(
        identifier, 
        5, 
        deleteEventId,
        deleteActionId,
        executionDate ?? DateTime.MinValue);