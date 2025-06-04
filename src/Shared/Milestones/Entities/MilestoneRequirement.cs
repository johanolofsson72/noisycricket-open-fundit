using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Shared.Milestones.DTOs;

namespace Shared.Milestones.Entities;

public class MilestoneRequirement
{
    public int RequirementIdentifier { get; set; } = 0;  
    public int RequirementTypeId { get; set; } = 0;
    public int DeliveryTypeId { get; set; } = 0;
    public int DocumentId { get; set; }  = 0;
    public bool IsApproved { get; set; }
    public bool IsDelivered { get; set; }
    public DateTime DeliveredDate { get; set; }
    public DateTime ApprovedDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ExpireDate { get; set; }
    [MaxLength(500)] public string Name { get; set; } = string.Empty;
    public virtual Milestone Milestone { get; set; } = new Milestone();
}

public static class MilestoneRequirementExtensions
{
    public static IEnumerable<MilestoneRequirementDto> ToDto(this IEnumerable<MilestoneRequirement> entities)
    {
        return entities.Select(entity => entity.ToDto());
    }
    
    public static IEnumerable<MilestoneRequirement> ToEntity(this IEnumerable<MilestoneRequirementDto> dtos)
    {
        return dtos.Select(dto => dto.ToEntity());
    }
    
    public static MilestoneRequirementDto ToDto(this MilestoneRequirement entity)
    {  
        return new MilestoneRequirementDto()
        {
            RequirementIdentifier = entity.RequirementIdentifier,
            RequirementTypeId = entity.RequirementTypeId,
            DeliveryTypeId = entity.DeliveryTypeId,
            DocumentId = entity.DocumentId,
            IsApproved = entity.IsApproved,
            IsDelivered = entity.IsDelivered,
            DeliveredDate = entity.DeliveredDate,
            ApprovedDate = entity.ApprovedDate,
            CreatedDate = entity.CreatedDate,
            ExpireDate = entity.ExpireDate
        };
    }
    
    public static MilestoneRequirement ToEntity(this MilestoneRequirementDto dto)
    {   
        return new MilestoneRequirement()
        {
            RequirementIdentifier = dto.RequirementIdentifier,
            RequirementTypeId = dto.RequirementTypeId,
            DeliveryTypeId = dto.DeliveryTypeId,
            DocumentId = dto.DocumentId,
            IsDelivered = dto.IsDelivered,
            DeliveredDate = dto.DeliveredDate,
            IsApproved = dto.IsApproved,
            ApprovedDate = dto.ApprovedDate,
            CreatedDate = dto.CreatedDate,
            ExpireDate = dto.ExpireDate
        };
    }
    
}







