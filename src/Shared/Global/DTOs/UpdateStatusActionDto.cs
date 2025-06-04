
using System;

namespace Shared.Global.DTOs;

public class UpdateStatusActionDto(
    int identifier, 
    int changeStatusToId, 
    DateTime? executionDate = null) 
    : ActionDto(
        identifier, 
        4,
        changeStatusToId,
        executionDate ?? DateTime.MinValue);