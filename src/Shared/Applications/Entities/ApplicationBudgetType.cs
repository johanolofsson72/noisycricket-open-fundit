using System.Collections.Generic;
using System.Linq;
using Shared.Global.DTOs;

namespace Shared.Applications.Entities;

public class ApplicationBudgetType
{
    public int Id { get; set; } = 0;
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
}

public static class ApplicationBudgetTypeExtensions
{
    public static List<ApplicationBudgetTypeDto> ToDto(this List<ApplicationBudgetType> entities)
    {
        return entities.Select(x => x.ToDto()).ToList();
    }

    public static List<ApplicationBudgetType> ToEntity(this List<ApplicationBudgetTypeDto> dtos)
    {
        return dtos.Select(x => x.ToEntity()).ToList();
    }
    
    public static ApplicationBudgetTypeDto ToDto(this ApplicationBudgetType entity) =>
        new ApplicationBudgetTypeDto
        {
            Id = entity.Id,
            Names = entity.Names
        };
    
    public static ApplicationBudgetType ToEntity(this ApplicationBudgetTypeDto dto) => new ApplicationBudgetType
    {
        Id = dto.Id,
        Names = dto.Names
    };
}