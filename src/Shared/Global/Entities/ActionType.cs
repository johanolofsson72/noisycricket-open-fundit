using System.Collections.Generic;
using System.Linq;
using Shared.Global.DTOs;

namespace Shared.Global.Entities;

public class ActionType
{
    public int Id { get; set; } = 0;
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
}

public static class ActionTypeExtensions
{
    public static IEnumerable<ActionTypeDto> ToDto(this IEnumerable<ActionType> entity)
    {
        return entity.Select(e => e.ToDto());
    }

    public static IEnumerable<ActionType> ToEntity(this IEnumerable<ActionTypeDto> dto)
    {
        return dto.Select(e => e.ToEntity()).ToList();
    }

    public static ActionTypeDto ToDto(this ActionType entity) =>
        new ()
        {
            Id = entity.Id,
            Names = entity.Names
        };
    
    public static ActionType ToEntity(this ActionTypeDto dto) => new()
    {
        Id = dto.Id,
        Names = dto.Names
    };
}