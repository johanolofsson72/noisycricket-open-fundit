namespace Shared.Users.DTOs;

public class UserOrganizationDto
{
    public int OrganizationIdentifier { get; set; } = 0;
    public string OrganizationName { get; set; } = string.Empty;
    public string OrganizationVat { get; set; } = string.Empty;
    public bool IsAdministrator { get; set; } = false;
}