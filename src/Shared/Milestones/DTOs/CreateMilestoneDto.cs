using System;

namespace Shared.Milestones.DTOs;

public class CreateMilestoneDto
{
    public int StatusId { get; init; } = 0;
    public decimal Amount { get; init; } = 0;
    public DateTime ExpireDate { get; init; }
    public bool IsLocked { get; init; }
    public List<MilestoneRequirementDto> Requirements { get; init; } = [];
    public List<MilestonePaymentDto> Payments { get; init; } = [];
}