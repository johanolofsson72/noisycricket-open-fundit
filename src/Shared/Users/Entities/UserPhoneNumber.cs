

using System.ComponentModel.DataAnnotations;
using Shared.Users.DTOs;

namespace Shared.Users.Entities;

public class UserPhoneNumber
{
    public int PhoneNumberIdentifier { get; set; }
    [MaxLength(500)] public string Number { get; set; } = string.Empty;
    public int Type { get; set; } = 0;
    public virtual User User { get; set; } = new User();
}

public static class UserPhoneNumberExtensions
{
    public static UserPhoneNumberDto ToDto(this UserPhoneNumber entity) =>
        new()
        {   
            PhoneNumberIdentifier = entity.PhoneNumberIdentifier,
            Number = entity.Number,
            Type = entity.Type
        };
    
    public static UserPhoneNumber ToEntity(this UserPhoneNumberDto dto) => 
        new()
        {   
            PhoneNumberIdentifier = dto.PhoneNumberIdentifier,
            Number = dto.Number,
            Type = dto.Type
        };
}



