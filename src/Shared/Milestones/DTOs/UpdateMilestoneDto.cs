using System;
using System.Collections.Generic;
using Shared.Global.Entities;

namespace Shared.Milestones.DTOs;

public record UpdateMilestoneDto
{
    public int Id { get; init; } = 0;
    public int StatusId { get; init; } = 0;
    public decimal Amount { get; init; } = 0;
    public DateTime ExpireDate { get; init; }
    public bool IsLocked { get; init; }
    public List<MilestoneRequirementDto> Requirements { get; init; } = [];
    public List<MilestonePaymentDto> Payments { get; init; } = [];
}