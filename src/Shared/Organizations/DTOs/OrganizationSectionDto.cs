namespace Shared.Organizations.DTOs;

public class OrganizationSectionDto
{
    public int OrganizationSectionIdentifier { get; set; } = 0;
    
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
    public int Order { get; set; } = 0;
    public bool Enabled { get; set; } = false;
}