
using System;

namespace Shared.Global.DTOs;

public class DeleteMessageActionDto(
    int identifier, 
    int deleteEventId,
    int deleteActionId,
    DateTime? executionDate = null) 
    : ActionDto(
        identifier, 
        5, 
        deleteEventId,
        deleteActionId,
        executionDate ?? DateTime.MinValue);