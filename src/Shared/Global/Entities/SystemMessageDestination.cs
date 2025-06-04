using System.Collections.Generic;
using System.Linq;
using Shared.Global.DTOs;

namespace Shared.Global.Entities;

public class SystemMessageDestination
{
    public int Id { get; set; } = 0;
    
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
}

public static class SystemMessageDestinationExtensions
{
    public static IEnumerable<SystemMessageDestinationDto> ToDto(this IEnumerable<SystemMessageDestination> entity)
    {
        return entity.Select(e => e.ToDto());
    }

    public static IEnumerable<SystemMessageDestination> ToEntity(this IEnumerable<SystemMessageDestinationDto> dto)
    {
        return dto.Select(e => e.ToEntity()).ToList();
    }
    public static SystemMessageDestinationDto ToDto(this SystemMessageDestination entity) =>
        new SystemMessageDestinationDto()
        {
            Id = entity.Id,
            Names = entity.Names
        };
    
    public static SystemMessageDestination ToEntity(this SystemMessageDestinationDto dto) => 
        new SystemMessageDestination()
        {
            Id = dto.Id,
            Names = dto.Names
        };
}