using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Milestones.DTOs;

namespace Shared.Milestones.Entities;

public class Milestone
{
    public int Id { get; set; } = 0;
    public int ApplicationId { get; set; } = 0;
    public int StatusId { get; set; } = 0;
    public decimal Amount { get; set; } = 0;
    public DateTime CreatedDate { get; set; }
    public DateTime ExpireDate { get; set; }
    public bool IsLocked { get; set; }
    public List<MilestoneRequirement> Requirements { get; set; } = new();
    public int RequirementsCount { get; set; } = 0;
    public int RequirementsDeliveredCount { get; set; } = 0;
    public bool RequirementsApproved { get; set; }
    public bool RequirementsExpired { get; set; }
    public List<MilestonePayment> Payments { get; set; } = new();
    public decimal TotalPayments { get; set; } = 0;
}

public static class MilestoneExtensions
{
    public static IEnumerable<MilestoneDto> ToDto(this IEnumerable<Milestone> entities)
    {
        return entities.Select(entity => entity.ToDto());
    }
    
    public static IEnumerable<Milestone> ToEntity(this IEnumerable<MilestoneDto> dtos)
    {
        return dtos.Select(dto => dto.ToEntity());
    }
    
    public static MilestoneDto ToDto(this Milestone entity)
    {
        return new MilestoneDto()
        {
            Id = entity.Id,
            ApplicationId = entity.ApplicationId,
            StatusId = entity.StatusId,
            Amount = entity.Amount,
            CreatedDate = entity.CreatedDate,
            ExpireDate = entity.ExpireDate,
            IsLocked = entity.IsLocked,
            Requirements = entity.Requirements.ToDto().ToList(),
            RequirementsCount = entity.RequirementsCount,
            RequirementsDeliveredCount = entity.RequirementsDeliveredCount,
            RequirementsApproved = entity.RequirementsApproved,
            RequirementsExpired = entity.RequirementsExpired,
            Payments = entity.Payments.ToDto().ToList(),
            TotalPayments = entity.TotalPayments
        };
    }
    
    public static Milestone ToEntity(this MilestoneDto dto)
    {   
        return new Milestone()
        {
            Id = dto.Id,
            ApplicationId = dto.ApplicationId,
            StatusId = dto.StatusId,
            Amount = dto.Amount,
            CreatedDate = dto.CreatedDate,
            ExpireDate = dto.ExpireDate,
            IsLocked = dto.IsLocked,
            Requirements = dto.Requirements.ToEntity().ToList(),
            RequirementsCount = dto.RequirementsCount,
            RequirementsDeliveredCount = dto.RequirementsDeliveredCount,
            RequirementsApproved = dto.RequirementsApproved,
            RequirementsExpired = dto.RequirementsExpired,
            Payments = dto.Payments.ToEntity().ToList(),
            TotalPayments = dto.TotalPayments
        };
    }
    
}



