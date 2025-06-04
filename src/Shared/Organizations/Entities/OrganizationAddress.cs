using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Shared.Organizations.DTOs;

namespace Shared.Organizations.Entities;

public class OrganizationAddress
{
    public int AddressIdentifier { get; set; } = 0;
    [MaxLength(500)] public string Line1 { get; set; } = string.Empty;
    [MaxLength(500)] public string Line2 { get; set; } = string.Empty;
    [MaxLength(500)] public string City { get; set; } = string.Empty;
    [MaxLength(500)] public string Country { get; set; } = string.Empty;
    [MaxLength(500)] public string PostalCode { get; set; } = string.Empty;
    public virtual Organization Organization { get; set; } = new Organization();
}


public static class OrganizationAddressExtensions
{

    public static IEnumerable<OrganizationAddressDto> ToDto(this IEnumerable<OrganizationAddress> entity)
    {
        return entity.Select(e => e.ToDto());
    }

    public static IEnumerable<OrganizationAddress> ToEntity(this IEnumerable<OrganizationAddressDto> dto)
    {
        return dto.Select(d => d.ToEntity());
    }
    public static OrganizationAddressDto ToDto(this OrganizationAddress entity)
    {
        return new OrganizationAddressDto()
        {
            AddressIdentifier = entity.AddressIdentifier,
            Line1 = entity.Line1,
            Line2 = entity.Line2,
            City = entity.City,
            Country = entity.Country,
            PostalCode = entity.PostalCode
        };
    }

    public static OrganizationAddress ToEntity(this OrganizationAddressDto dto)
    {
        return new OrganizationAddress
        {
            AddressIdentifier = dto.AddressIdentifier,
            Line1 = dto.Line1,
            Line2 = dto.Line2,
            City = dto.City,
            Country = dto.Country,
            PostalCode = dto.PostalCode
        };
    }
}



