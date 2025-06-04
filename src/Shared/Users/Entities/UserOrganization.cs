using System.ComponentModel.DataAnnotations;
using Shared.Users.DTOs;

namespace Shared.Users.Entities
{
    public class UserOrganization
    {
        public int OrganizationIdentifier { get; set; } = 0;
        [MaxLength(500)] public string OrganizationName { get; set; } = string.Empty;
        [MaxLength(500)] public string OrganizationVat { get; set; } = string.Empty;
        public bool IsAdministrator { get; set; }
        public virtual User User { get; set; } = new User();
    }

    public static class UserOrganizationExtensions
    {
        public static UserOrganizationDto ToDto(this UserOrganization entity) =>
            new()
            {   
                OrganizationIdentifier = entity.OrganizationIdentifier,
                OrganizationName = entity.OrganizationName,
                OrganizationVat = entity.OrganizationVat,
                IsAdministrator = entity.IsAdministrator
            };
    
        public static UserOrganization ToEntity(this UserOrganizationDto dto) => 
            new()
            {
                OrganizationIdentifier = dto.OrganizationIdentifier,
                OrganizationName = dto.OrganizationName,
                OrganizationVat = dto.OrganizationVat,
                IsAdministrator = dto.IsAdministrator
            };
    }


}



