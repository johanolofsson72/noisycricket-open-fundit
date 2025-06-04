
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Shared.Organizations.DTOs;

namespace Shared.Organizations.Entities;

public class OrganizationPhoneNumber
{
    public int PhoneNumberIdentifier { get; set; } = 0;
    [MaxLength(500)] public string Number { get; set; } = string.Empty;
    public int Type { get; set; } = 0;
    public virtual Organization Organization { get; set; } = new Organization();
}



public static class OrganizationPhoneNumberExtensions
{

    public static IEnumerable<OrganizationPhoneNumberDto> ToDto(this IEnumerable<OrganizationPhoneNumber> entity)
    {
        return entity.Select(e => e.ToDto());
    }

    public static IEnumerable<OrganizationPhoneNumber> ToEntity(this IEnumerable<OrganizationPhoneNumberDto> dto)
    {
        return dto.Select(d => d.ToEntity());
    }
    public static OrganizationPhoneNumberDto ToDto(this OrganizationPhoneNumber entity)
    {
        return new OrganizationPhoneNumberDto()
        {
            PhoneNumberIdentifier = entity.PhoneNumberIdentifier,
            Number = entity.Number,
            Type = entity.Type
        };
    }

    public static OrganizationPhoneNumber ToEntity(this OrganizationPhoneNumberDto dto)
    {
        return new OrganizationPhoneNumber
        {
            PhoneNumberIdentifier = dto.PhoneNumberIdentifier,
            Number = dto.Number,
            Type = dto.Type
        };
    }
}


