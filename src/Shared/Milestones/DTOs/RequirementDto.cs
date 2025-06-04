using System;

namespace Shared.Milestones.DTOs;

public class RequirementDto
{
    public int Id { get; init; }
    public int DocumentTypeId { get; init; } 
    public int DocumentId { get; init; } 
    public bool IsDelivered { get; init; } 
    public bool IsApproved { get; init; }
    public bool IsSentExternal { get; init; }
    public bool CanSendExternal { get; init; }
    public DateTime DeliveredDate { get; init; } 
    public DateTime ApprovedDate { get; init; }
    public DateTime SentExternalDate { get; init; }
    public DateTime CreatedDate { get; init; }
    public DateTime ExpireDate { get; init; }

}