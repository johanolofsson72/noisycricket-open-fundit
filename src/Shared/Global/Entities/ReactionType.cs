using System.Collections.Generic;
using System.Linq;
using Shared.Global.DTOs;

namespace Shared.Global.Entities;

public class ReactionType
{
    public int Id { get; set; } = 0;
    
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
    
    public List<string> Messages { get; set; } = ["", "", "", "", "", "", "", ""];
}

public static class ReactionTypeExtensions
{
    public static IEnumerable<ReactionTypeDto> ToDto(this IEnumerable<ReactionType> entity)
    {
        return entity.Select(e => e.ToDto());
    }

    public static IEnumerable<ReactionType> ToEntity(this IEnumerable<ReactionTypeDto> dto)
    {
        return dto.Select(e => e.ToEntity()).ToList();
    }
    
    public static ReactionTypeDto ToDto(this ReactionType entity) =>
        new ()
        {
            Id = entity.Id,
            Names = entity.Names,
            Messages = entity.Messages
        };
    
    public static ReactionType ToEntity(this ReactionTypeDto dto) => new()
    {
        Id = dto.Id,
        Names = dto.Names,
        Messages = dto.Messages
    };
}