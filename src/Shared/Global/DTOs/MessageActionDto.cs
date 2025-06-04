
using System;

namespace Shared.Global.DTOs;

public class MessageActionDto(
    int identifier, 
    int receiverClaimTypeId, 
    int systemDestination, 
    string systemMessage,
    DateTime? executionDate = null,
    int? reactionDescription = null) 
    : ActionDto(
        identifier, 
        2,
        receiverClaimTypeId,
        systemDestination,
        systemMessage,
        executionDate ?? DateTime.MinValue,
        reactionDescription ?? 0);

public class MessageActionWithLinkDto(
    int identifier, 
    int receiverClaimTypeId, 
    int systemDestination, 
    string systemMessage,
    string documentLink,
    DateTime? executionDate = null) 
    : ActionDto(
        identifier, 
        2,
        receiverClaimTypeId,
        systemDestination,
        systemMessage,
        documentLink,
        executionDate ?? DateTime.MinValue);