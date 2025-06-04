using System.Collections.Generic;
using System.Linq;
using Shared.Global.DTOs;

namespace Shared.Global.Entities;

public class Gender
{
    public int Id { get; set; } = 0;
    
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
}

public static class GenderExtensions
{
    public static IEnumerable<GenderDto> ToDto(this IEnumerable<Gender> entity)
    {
        return entity.Select(e => e.ToDto());
    }

    public static IEnumerable<Gender> ToEntity(this IEnumerable<GenderDto> dto)
    {
        return dto.Select(e => e.ToEntity()).ToList();
    }

    public static GenderDto ToDto(this Gender entity) =>
        new ()
        {
            Id = entity.Id,
            Names = entity.Names
        };
    
    public static Gender ToEntity(this GenderDto dto) => new()
    {
        Id = dto.Id,
        Names = dto.Names
    };
}