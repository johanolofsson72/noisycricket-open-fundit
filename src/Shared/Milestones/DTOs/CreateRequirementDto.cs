using System;

namespace Shared.Milestones.DTOs;

public record CreateRequirementDto(
    int MilestoneId,
    int DocumentTypeId,
    bool IsDelivered,
    bool IsApproved,
    bool IsSentExternal,
    bool CanSendExternal,
    DateTime DeliveredDate,
    DateTime ApprovedDate,
    DateTime SentExternalDate,
    DateTime ExpireDate
);