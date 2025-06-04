namespace Shared.Projects.DTOs;

public class ProjectOrganizationDto
{
    public int OrganizationIdentifier { get; set; }
    public string OrganizationTitle { get; set; } = string.Empty;
    public string OrganizationVat { get; set; } = string.Empty;
    public string OrganizationEmail { get; set; } = string.Empty;
    public string OrganizationUrl { get; set; } = string.Empty;
    public string OrganizationAddress { get; set; } = string.Empty;
    public string OrganizationCity { get; set; } = string.Empty;
    public string OrganizationPostalCode { get; set; } = string.Empty;
    public string OrganizationCountry { get; set; } = string.Empty;
    public string OrganizationPhoneNumber { get; set; } = string.Empty;
    public string OrganizationLogo { get; set; } = string.Empty;
}