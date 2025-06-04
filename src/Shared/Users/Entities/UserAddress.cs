using System.ComponentModel.DataAnnotations;
using Shared.Users.DTOs;

namespace Shared.Users.Entities;

public class UserAddress
{
    public int AddressIdentifier { get; set; }
    [MaxLength(500)] public string Line1 { get; set; } = string.Empty;
    [MaxLength(500)] public string Line2 { get; set; } = string.Empty;
    [MaxLength(500)] public string City { get; set; } = string.Empty;
    [MaxLength(500)] public string Country { get; set; } = string.Empty;
    [MaxLength(500)] public string PostalCode { get; set; } = string.Empty;
    public virtual User User { get; set; } = new User();
}

public static class UserAddressExtensions
{
    public static UserAddressDto ToDto(this UserAddress entity) =>
        new()
        {   
            AddressIdentifier = entity.AddressIdentifier,
            Line1 = entity.Line1,
            Line2 = entity.Line2,
            City = entity.City,
            Country = entity.Country,
            PostalCode = entity.PostalCode
        };
    
    public static UserAddress ToEntity(this UserAddressDto dto) => 
        new()
        {
            AddressIdentifier = dto.AddressIdentifier,
            Line1 = dto.Line1,
            Line2 = dto.Line2,
            City = dto.City,
            Country = dto.Country,
            PostalCode = dto.PostalCode
        };
}

