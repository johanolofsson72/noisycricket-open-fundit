
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Shared.Controls.DTOs;

namespace Shared.Controls.Entities;

public class ControlType
{
    public int Id { get; set; }
    [MaxLength(500)] public string Name { get; set; } = string.Empty;
}

public static class ControlTypeExtensions
{
    public static List<ControlTypeDto> ToDto(this List<ControlType> entities)
    {
        return entities.Select(x => x.ToDto()).ToList();
    }

    public static List<ControlType> ToEntity(this List<ControlTypeDto> dtos)
    {
        return dtos.Select(x => x.ToEntity()).ToList();
    }
    
    public static ControlTypeDto ToDto(this ControlType entity) =>
        new ControlTypeDto()
        {
            Id = entity.Id,
            Name = entity.Name
        };
    
    public static ControlType ToEntity(this ControlTypeDto dto) => 
        new ControlType()
        {
            Id = dto.Id,
            Name = dto.Name
        };
}