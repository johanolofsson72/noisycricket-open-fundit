using System.Collections.Generic;
using System.Linq;
using Shared.Global.DTOs;

namespace Shared.Global.Entities;

public class MessageType
{
    public int Id { get; set; } = 0;
    
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
}

public static class MessageTypeExtensions
{
    public static IEnumerable<MessageTypeDto> ToDto(this IEnumerable<MessageType> entity)
    {
        return entity.Select(e => e.ToDto());
    }

    public static IEnumerable<MessageType> ToEntity(this IEnumerable<MessageTypeDto> dto)
    {
        return dto.Select(e => e.ToEntity()).ToList();
    }

    public static MessageTypeDto ToDto(this MessageType entity) =>
        new ()
        {
            Id = entity.Id,
            Names = entity.Names
        };
    
    public static MessageType ToEntity(this MessageTypeDto dto) => new()
    {
        Id = dto.Id,
        Names = dto.Names
    };
}