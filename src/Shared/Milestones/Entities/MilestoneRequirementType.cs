using System.Collections.Generic;
using System.Linq;
using Shared.Milestones.DTOs;

namespace Shared.Milestones.Entities;

public class MilestoneRequirementType
{
    public int Id { get; set; } = 0;
    
    
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
}

public static class MilestoneRequirementTypeExtensions
{
    public static IEnumerable<MilestoneRequirementTypeDto> ToDto(this IEnumerable<MilestoneRequirementType> entities)
    {
        return entities.Select(entity => entity.ToDto());
    }
    
    public static IEnumerable<MilestoneRequirementType> ToEntity(this IEnumerable<MilestoneRequirementTypeDto> dtos)
    {
        return dtos.Select(dto => dto.ToEntity());
    }
    
    public static MilestoneRequirementTypeDto ToDto(this MilestoneRequirementType entity)
    {  
        return new MilestoneRequirementTypeDto()
        {
            Id = entity.Id,
            Names = entity.Names
        };
    }
    
    public static MilestoneRequirementType ToEntity(this MilestoneRequirementTypeDto dto)
    {   
        return new MilestoneRequirementType()
        {
            Id = dto.Id,
            Names = dto.Names
        };
    }
    
}







