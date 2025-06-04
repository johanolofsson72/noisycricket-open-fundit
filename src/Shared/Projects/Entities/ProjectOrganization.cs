using System.ComponentModel.DataAnnotations;

namespace Shared.Projects.Entities;

public class ProjectOrganization
{
    public int OrganizationIdentifier { get; set; } = 0;
    [MaxLength(500)] public string OrganizationName { get; set; } = string.Empty;
    [MaxLength(500)] public string OrganizationVat { get; set; } = string.Empty;
    [MaxLength(500)] public string OrganizationEmail { get; set; } = string.Empty;
    [MaxLength(500)] public string OrganizationUrl { get; set; } = string.Empty;
    [MaxLength(500)] public string OrganizationAddress { get; set; } = string.Empty;
    [MaxLength(500)] public string OrganizationCity { get; set; } = string.Empty;
    [MaxLength(500)] public string OrganizationPostalCode { get; set; } = string.Empty;
    [MaxLength(500)] public string OrganizationCountry { get; set; } = string.Empty;
    [MaxLength(500)] public string OrganizationPhoneNumber { get; set; } = string.Empty;
    [MaxLength(250)] public string OrganizationLogo { get; set; } = string.Empty;
}