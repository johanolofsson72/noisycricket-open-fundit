namespace Shared.Organizations.DTOs;

public class OrganizationPhoneNumberTypeDto
{
    public int OrganizationPhoneNumberTypeIdentifier { get; set; } = 0;
    
    public List<string> Names { get; set; } = ["", "", "", "", "", "", "", ""];
    public string First => Names.Count > 0 ? Names[0] : "Unknown";
}