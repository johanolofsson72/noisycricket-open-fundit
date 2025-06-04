
using System;

namespace Shared.Global.DTOs;

public class EmailActionDto(
    int identifier, 
    int receiverClaimTypeId, 
    string emailMessageBody, 
    DateTime? executionDate = null,
    int? reactionDescriptionId = null) 
    : ActionDto(
        identifier, 
        3, 
        receiverClaimTypeId,
        emailMessageBody,
        executionDate ?? DateTime.MinValue,
        reactionDescriptionId ?? 0);

public class EmailActionWithLinkDto(
    int identifier, 
    int receiverClaimTypeId, 
    string emailMessageBody, 
    string documentLink,
    DateTime? executionDate = null) 
    : ActionDto(
        identifier, 
        3, 
        receiverClaimTypeId,
        emailMessageBody,
        documentLink,
        executionDate ?? DateTime.MinValue);