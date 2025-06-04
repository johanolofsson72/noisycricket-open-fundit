using System.Collections.Generic;
using System.Linq;
using Shared.Global.DTOs;

namespace Shared.Global.Entities;

public class PhoneNumberType
{
    public int Id { get; set; } = 0;
    
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
}

public static class PhoneNumberTypeExtensions
{
    public static IEnumerable<PhoneNumberTypeDto> ToDto(this IEnumerable<PhoneNumberType> entity)
    {
        return entity.Select(e => e.ToDto());
    }

    public static IEnumerable<PhoneNumberType> ToEntity(this IEnumerable<PhoneNumberTypeDto> dto)
    {
        return dto.Select(e => e.ToEntity()).ToList();
    }

    public static PhoneNumberTypeDto ToDto(this PhoneNumberType entity) =>
        new ()
        {
            Id = entity.Id,
            Names = entity.Names
        };
    
    public static PhoneNumberType ToEntity(this PhoneNumberTypeDto dto) => new()
    {
        Id = dto.Id,
        Names = dto.Names
    };
}