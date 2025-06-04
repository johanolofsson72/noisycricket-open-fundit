using System.Collections.Generic;
using System.Linq;
using Shared.Global.DTOs;

namespace Shared.Global.Entities;

public class Section
{
    public int Id { get; set; } = 0;
    
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
    public int Order { get; set; } = 0;
    public bool Enabled { get; set; } = false;
}

public static class SectionExtensions
{
    public static IEnumerable<SectionDto> ToDto(this IEnumerable<Section> entity)
    {
        return entity.Select(e => e.ToDto());
    }

    public static IEnumerable<Section> ToEntity(this IEnumerable<SectionDto> dto)
    {
        return dto.Select(e => e.ToEntity()).ToList();
    }
    
    public static SectionDto ToDto(this Section entity) =>
        new ()
        {
            Id = entity.Id,
            Names = entity.Names,
            Order = entity.Order,
            Enabled = entity.Enabled
        };
    
    public static Section ToEntity(this SectionDto dto) => new()
    {
        Id = dto.Id,
        Names= dto.Names,
        Order = dto.Order,
        Enabled = dto.Enabled
    };
}