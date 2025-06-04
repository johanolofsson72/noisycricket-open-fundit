using System;
using System.Collections.Generic;

namespace Shared.Milestones.DTOs;

public class MilestoneDto
{
    public int Id { get; set; } = 0;
    public int ApplicationId { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public decimal Amount { get; set; } = 0;
    public DateTime CreatedDate { get; set; }
    public DateTime ExpireDate { get; set; }
    public bool IsLocked { get; set; }
    public List<MilestoneRequirementDto> Requirements { get; set; } = new();
    public int RequirementsCount { get; set; } = 0;
    public int RequirementsDeliveredCount { get; set; } = 0;
    public bool RequirementsApproved { get; set; }
    public bool RequirementsExpired { get; set; }
    public List<MilestonePaymentDto> Payments { get; set; } = new();
    public decimal TotalPayments { get; set; } = 0;
}