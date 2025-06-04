using System.ComponentModel.DataAnnotations;
using Shared.Applications.DTOs;

namespace Shared.Applications.Entities;

public class ApplicationContact
{
    public int ContactIdentifier { get; set; } = 0;
    [MaxLength(500)] public string Name { get; set; } = string.Empty;
    [MaxLength(500)] public string Email { get; set; } = string.Empty;
    [MaxLength(500)] public string PhoneNumber { get; set; } = string.Empty;
}
    
public static class ApplicationContactExtensions
{
    public static ApplicationContactDto ToDto(this ApplicationContact entity)
    {
        return new ApplicationContactDto(){Id = entity.ContactIdentifier, Name = entity.Name, Email = entity.Email, PhoneNumber = entity.PhoneNumber};
    }
    
    public static ApplicationContact ToEntity(this ApplicationContactDto dto)
    {
        return new ApplicationContact
        {
            ContactIdentifier = dto.Id,
            Name = dto.Name,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber
        };
    }
}