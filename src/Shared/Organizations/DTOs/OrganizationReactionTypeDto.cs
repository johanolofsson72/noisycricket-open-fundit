namespace Shared.Organizations.DTOs;

public class OrganizationReactionTypeDto
{
    public int OrganizationReactionTypeIdentifier { get; set; } = 0;
    
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
    public List<string> Messages { get; set; } = ["", "", "", "", "", "", "", ""];
}