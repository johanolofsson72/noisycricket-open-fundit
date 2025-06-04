namespace Shared.Organizations.DTOs;

public class OrganizationProjectDto
{
    public int Id { get; set; } = 0;
    public int OrganizationProjectIdentifier { get; set; } = 0;
    public List <string> Names { get; set; } = [];
}