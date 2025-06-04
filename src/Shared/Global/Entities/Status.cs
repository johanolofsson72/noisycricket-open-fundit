using System.Collections.Generic;
using System.Linq;
using Shared.Global.DTOs;

namespace Shared.Global.Entities;

public class Status
{
    public int Id { get; set; } = 0;
    
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
}

public static class StatusExtensions
{
    public static IEnumerable<StatusDto> ToDto(this IEnumerable<Status> entity)
    {
        return entity.Select(e => e.ToDto());
    }

    public static IEnumerable<Status> ToEntity(this IEnumerable<StatusDto> dto)
    {
        return dto.Select(e => e.ToEntity()).ToList();
    }
    
    public static StatusDto ToDto(this Status entity) =>
        new ()
        {
            Id = entity.Id,
            Names = entity.Names
        };
    
    public static Status ToEntity(this StatusDto dto) => new()
    {
        Id = dto.Id,
        Names = dto.Names
    };
}