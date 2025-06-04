namespace Shared.Organizations.DTOs;

public class OrganizationClaimTypeDto
{
    public int OrganizationClaimTypeIdentifier { get; set; } = 0;
    
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
    public string Tag { get; set; } = string.Empty;
}