using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Shared.Global.DTOs;

namespace Shared.Global.Entities;

public class ClaimType
{
    public int Id { get; set; } = 0;
    
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
    [MaxLength(5)] public string Tag { get; set; } = string.Empty;
}

public static class ClaimTypeExtensions
{
    public static IEnumerable<ClaimTypeDto> ToDto(this IEnumerable<ClaimType> entity)
    {
        return entity.Select(e => e.ToDto());
    }

    public static IEnumerable<ClaimType> ToEntity(this IEnumerable<ClaimTypeDto> dto)
    {
        return dto.Select(e => e.ToEntity()).ToList();
    }

    public static ClaimTypeDto ToDto(this ClaimType entity) =>
        new ()
        {
            Id = entity.Id,
            Names = entity.Names,
            Tag = entity.Tag
        };
    
    public static ClaimType ToEntity(this ClaimTypeDto dto) => new()
    {
        Id = dto.Id,
        Names = dto.Names,
        Tag = dto.Tag
    };
}