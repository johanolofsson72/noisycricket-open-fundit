using System;
using Shared.Documents.Entities;

namespace Shared.Milestones.DTOs;

public record UpdateRequirementDto(
    Guid Id,
    DocumentType DocumentType,
    int DocumentId,
    bool IsDelivered,
    bool IsApproved,
    bool IsSentExternal,
    bool CanSendExternal,
    DateTime DeliveredDate,
    DateTime ApprovedDate,
    DateTime SentExternalDate,
    DateTime ExpireDate
);