namespace Shared.Projects.DTOs;

public class CreateProjectDto
{
    public int OrganizationId { get; set; }
    public string Number { get; set; } = string.Empty;
    public List<string> Titles { get; set; } = [];
}