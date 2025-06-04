using System.Collections.Generic;
using System.Linq;
using Shared.Global.DTOs;

namespace Shared.Global.Entities;

public class EventType
{
    public int Id { get; set; }
    
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
}

public static class EventTypeExtensions
{
    public static IEnumerable<EventTypeDto> ToDto(this IEnumerable<EventType> entity)
    {
        return entity.Select(e => e.ToDto());
    }

    public static IEnumerable<EventType> ToEntity(this IEnumerable<EventTypeDto> dto)
    {
        return dto.Select(e => e.ToEntity()).ToList();
    }

    public static EventTypeDto ToDto(this EventType entity) =>
        new ()
        {
            Id = entity.Id,
            Names = entity.Names
        };
    
    public static EventType ToEntity(this EventTypeDto dto) => new()
    {
        Id = dto.Id,
        Names = dto.Names
    };
}